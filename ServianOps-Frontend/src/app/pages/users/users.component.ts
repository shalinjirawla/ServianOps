import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { UserDetailDto, UsersService, RolesService, RoleLookupDto } from '../../core/api/service-proxies';
import { Router } from '@angular/router';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss',
})
export class UsersComponent implements OnInit {
  searchQuery = '';
  rawUsers: any[] = [];
  rolesList: RoleLookupDto[] = [];

  showDrawer = false;
  isEditMode = false;
  selectedUserId: number | null = null;
  submitted = false;

  userForm!: FormGroup;

  constructor(
    private userService: UsersService,
    private rolesService: RolesService,
    private auth: AuthService,
    private toast: ToastService,
    private router: Router,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    if (this.auth.userRole() === 'SuperAdmin') {
      this.router.navigate(['/tenants']);
      return;
    }
    this.loadRoles();
    this.loadUsers();
    this.initForm();
  }

  initForm() {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^(\\+?[0-9\\s\\-\\(\\)]{7,20})?$')]],
      roleId: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  loadRoles() {
    this.rolesService.getRoleLookup().subscribe({
      next: (res) => {
        this.rolesList = res.data || [];
      },
      error: () => {
        this.toast.error('Failed to load user roles');
      }
    });
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.rawUsers = data.data?.items || [];
      },
      error: () => {
        this.toast.error('Failed to load users');
      }
    });
  }

  // Pagination State
  currentPage = 1;
  pageSize = 5;
  mathMin = Math.min;

  // Sorting State
  sortField = 'firstName';
  sortAsc = true;

  get filteredAndSortedUsers() {
    let list = [...this.rawUsers];

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(
        (u) =>
          (u.firstName || '').toLowerCase().includes(q) ||
          (u.lastName || '').toLowerCase().includes(q) ||
          (u.email || '').toLowerCase().includes(q)
      );
    }

    // 2. Sorting
    list.sort((a, b) => {
      let valA: any = (a as any)[this.sortField] || '';
      let valB: any = (b as any)[this.sortField] || '';

      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();

      if (valA < valB) return this.sortAsc ? -1 : 1;
      if (valA > valB) return this.sortAsc ? 1 : -1;
      return 0;
    });

    return list;
  }

  get users() {
    const list = this.filteredAndSortedUsers;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedUsers.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  openCreateDrawer() {
    this.isEditMode = false;
    this.selectedUserId = null;
    this.submitted = false;

    this.userForm.reset({
      roleId: ''
    });
    this.userForm.get('email')?.enable();
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.userForm.get('roleId')?.setValidators([Validators.required]);
    this.userForm.get('roleId')?.updateValueAndValidity();

    this.showDrawer = true;
  }

  openEditDrawer(u: any) {
    this.isEditMode = true;
    this.selectedUserId = u.id;
    this.submitted = false;

    const userRoleName = u.roles && u.roles.length > 0 ? u.roles[0] : '';
    const matchingRole = this.rolesList.find(r => r.name === userRoleName);

    this.userForm.patchValue({
      firstName: u.firstName,
      lastName: u.lastName,
      email: u.email,
      phone: u.phone,
      roleId: matchingRole ? matchingRole.id : '',
      password: 'N/A' // dummy value to satisfy form
    });
    this.userForm.get('email')?.disable();
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
    this.userForm.get('roleId')?.setValidators([Validators.required]);
    this.userForm.get('roleId')?.updateValueAndValidity();

    this.showDrawer = true;
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.userForm.reset();
  }

  saveUser() {
    this.submitted = true;
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const payload = this.userForm.getRawValue();

    if (this.isEditMode && this.selectedUserId) {
      this.userService.updateUser(this.selectedUserId, {
        firstName: payload.firstName,
        lastName: payload.lastName,
        phone: payload.phone,
        email: payload.email,
        roleIds: [Number(payload.roleId)],
      }).subscribe({
        next: () => {
          this.toast.success('User updated successfully');
          this.loadUsers();
          this.closeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to update user');
        }
      });
    } else {
      this.userService.createUser({
        firstName: payload.firstName,
        lastName: payload.lastName,
        email: payload.email,
        phone: payload.phone,
        password: payload.password,
        roleIds: [Number(payload.roleId)],
      }).subscribe({
        next: () => {
          this.toast.success('User created successfully');
          this.loadUsers();
          this.closeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to create user');
        }
      });
    }
  }

  deleteUser(id: number) {
    this.confirmSvc.confirm(
      'Delete User Account',
      'Are you sure you want to delete this user? This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.userService.deleteUser(id).subscribe({
          next: () => {
            this.toast.success('User deleted successfully');
            this.loadUsers();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete user');
          }
        });
      }
    });
  }

  toggleStatus(u: any) {
    this.userService.toggleUserStatus(u.id).subscribe({
      next: () => {
        this.toast.success(`User status updated`);
        this.loadUsers();
      },
      error: (err) => {
        this.toast.error(err.error?.error || 'Failed to toggle status');
      }
    });
  }

  initials(name: string): string {
    return name ? name.split(' ').map((n) => n[0]).join('') : '';
  }
}
