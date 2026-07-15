import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { CustomersService, CustomerTypesService, CustomerListDto } from '../../core/api/service-proxies';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './clients.component.html',
  styleUrl: './clients.component.scss',
})
export class ClientsComponent implements OnInit {
  private customersService = inject(CustomersService);
  private customerTypesService = inject(CustomerTypesService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);
  private confirmSvc = inject(ConfirmationModalService);

  rawClients: CustomerListDto[] = [];
  showDrawer = false;
  submitted = false;
  isEditMode = false;
  selectedClientId: number | null = null;

  clientForm!: FormGroup;
  defaultCustomerTypeId = 0;

  // Pagination State
  currentPage = 1;
  pageSize = 10;
  mathMin = Math.min;

  // Sorting State
  sortField = 'companyName';
  sortAsc = true;

  // Filter State
  searchQuery = '';

  ngOnInit() {
    this.initForm();
    this.ensureDefaultCustomerTypeThenLoad();
  }

  initForm() {
    this.clientForm = this.fb.group({
      name: ['', [Validators.required]],
      contact: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^(\\+?[0-9\\s\\-\\(\\)]{7,20})?$')]],
      billingAddress: [''],
      billingEmail: ['', [Validators.email]],
      terms: ['Net 30', [Validators.required]],
      vat: [''],
      hourlyRate: [80, [Validators.required, Validators.min(0)]],
      calloutFee: [120, [Validators.required, Validators.min(0)]],
      outOfHoursMultiplier: [1.5, [Validators.required, Validators.min(1)]],
      slaP1: [4, [Validators.required, Validators.min(1)]],
      slaP2: [24, [Validators.required, Validators.min(1)]],
      slaP3: [3, [Validators.required, Validators.min(1)]],
      slaP4: [7, [Validators.required, Validators.min(1)]],
      poRequired: [false],
      poLimit: [1000, [Validators.min(0)]],
      emailInvoice: [true],
      autoNotify: [true]
    });
  }

  ensureDefaultCustomerTypeThenLoad() {
    this.customerTypesService.getAllCustomerTypes().subscribe({
      next: (res) => {
        const types = res.data?.items;
        if (types && types.length > 0) {
          this.defaultCustomerTypeId = types[0].id!;
          this.loadClients();
        } else {
          this.customerTypesService.createCustomerType({ name: 'General' }).subscribe({
            next: (newType) => {
              this.defaultCustomerTypeId = newType.data?.id!;
              this.loadClients();
            },
            error: () => {
              this.toast.error('Failed to initialize client types');
            }
          });
        }
      },
      error: () => {
        this.toast.error('Failed to query client types');
      }
    });
  }

  loadClients() {
    this.customersService.getAllCustomers(undefined, undefined, undefined, undefined, undefined, undefined, 1, 200).subscribe({
      next: (res) => {
        this.rawClients = res.data?.items || [];
      },
      error: () => {
        this.toast.error('Failed to load clients from backend');
      }
    });
  }

  get filteredAndSortedClients() {
    let list = this.rawClients.map(c => ({
      id: `CL-${String(c.id).padStart(3, '0')}`,
      rawId: c.id,
      name: c.companyName || c.name || 'Unnamed Client',
      contact: c.primaryContactName || 'No Contact',
      email: c.primaryContactName ? `${c.primaryContactName.toLowerCase().replace(/\s+/g, '.')}@client.com` : 'info@client.com',
      phone: c.primaryContactMobile || c.mobileNumber || 'N/A',
      sites: 0,
      openJobs: 0,
      terms: 'Net 30',
      vat: 'Exempt',
      companyName: c.companyName || c.name || '',
      primaryContactName: c.primaryContactName || '',
    }));

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(c =>
        (c.companyName && c.companyName.toLowerCase().includes(q)) ||
        (c.name && c.name.toLowerCase().includes(q)) ||
        (c.contact && c.contact.toLowerCase().includes(q)) ||
        (c.phone && c.phone.toLowerCase().includes(q))
      );
    }

    // 2. Sorting
    list.sort((a, b) => {
      let valA: any = '';
      let valB: any = '';

      if (this.sortField === 'companyName' || this.sortField === 'name') {
        valA = a.companyName || a.name || '';
        valB = b.companyName || b.name || '';
      } else if (this.sortField === 'primaryContactName' || this.sortField === 'contact') {
        valA = a.contact || '';
        valB = b.contact || '';
      } else if (this.sortField === 'id') {
        valA = a.rawId;
        valB = b.rawId;
      } else {
        valA = (a as any)[this.sortField] || '';
        valB = (b as any)[this.sortField] || '';
      }

      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();

      if (valA < valB) return this.sortAsc ? -1 : 1;
      if (valA > valB) return this.sortAsc ? 1 : -1;
      return 0;
    });

    return list;
  }

  get clients() {
    const list = this.filteredAndSortedClients;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedClients.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  openDrawer() {
    this.isEditMode = false;
    this.selectedClientId = null;
    this.submitted = false;
    this.clientForm.reset({
      terms: 'Net 30',
      hourlyRate: 80,
      calloutFee: 120,
      outOfHoursMultiplier: 1.5,
      slaP1: 4,
      slaP2: 24,
      slaP3: 3,
      slaP4: 7,
      poRequired: false,
      poLimit: 1000,
      emailInvoice: true,
      autoNotify: true
    });
    this.showDrawer = true;
  }

  openEditDrawer(c: any) {
    this.isEditMode = true;
    this.selectedClientId = c.rawId;
    this.submitted = false;

    this.customersService.getCustomerById(c.rawId).subscribe({
      next: (res) => {
        const detail = res.data!;
        const contact = detail.contacts && detail.contacts.length > 0 ? detail.contacts[0] : null;
        this.clientForm.patchValue({
          name: detail.companyName || detail.name,
          contact: contact ? `${contact.firstName} ${contact.lastName}`.trim() : '',
          email: contact ? contact.email : '',
          phone: contact ? contact.mobileNumber : detail.mobileNumber,
          billingAddress: detail.area,
          billingEmail: contact ? contact.email : '',
          terms: `Net ${detail.paymentTerms}`,
          vat: detail.vatNumber,
          poRequired: detail.isPORequired,
          hourlyRate: 80,
          calloutFee: 120,
          outOfHoursMultiplier: 1.5,
          slaP1: 4,
          slaP2: 24,
          slaP3: 3,
          slaP4: 7,
          poLimit: 1000,
          emailInvoice: true,
          autoNotify: true
        });
        this.showDrawer = true;
      },
      error: () => {
        this.toast.error('Failed to load client details');
      }
    });
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.clientForm.reset();
  }

  saveClient() {
    this.submitted = true;
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }

    const val = this.clientForm.value;
    const nameParts = (val.contact || '').trim().split(/\s+/);
    const firstName = nameParts[0] || '';
    const lastName = nameParts.slice(1).join(' ') || '';

    const payload = {
      name: val.name,
      companyName: val.name,
      area: val.billingAddress || '',
      city: 'London',
      countryOrState: 'UK',
      postCode: 'EC1A',
      mobileNumber: val.phone || '',
      accountNumber: val.vat || '',
      paymentTerms: Number(val.terms.replace('Net ', '')) || 30,
      isVatRegistered: val.vat && val.vat !== 'Exempt' ? true : false,
      vatNumber: val.vat || '',
      isPORequired: val.poRequired || false,
      customerTypeId: this.defaultCustomerTypeId,
      contactFirstName: firstName,
      contactLastName: lastName,
      contactMobile: val.phone || '',
      contactEmail: val.email || ''
    };

    if (this.isEditMode && this.selectedClientId) {
      this.customersService.updateCustomer(this.selectedClientId, payload).subscribe({
        next: () => {
          this.toast.success('Client profile updated successfully');
          this.loadClients();
          this.closeDrawer();
        },
        error: () => {
          this.toast.error('Failed to update client');
        }
      });
    } else {
      this.customersService.createCustomer(payload).subscribe({
        next: () => {
          this.toast.success('Client profile created successfully');
          this.loadClients();
          this.closeDrawer();
        },
        error: () => {
          this.toast.error('Failed to create client');
        }
      });
    }
  }

  deleteClient(id: any) {
    this.confirmSvc.confirm(
      'Delete Client Profile',
      'Are you sure you want to delete this client? This will also remove any associated historical records. This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.customersService.deleteCustomer(id).subscribe({
          next: () => {
            this.toast.success('Client profile deleted successfully');
            this.loadClients();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete client');
          }
        });
      }
    });
  }
}
