import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { SiteService, SiteListDto } from '../../core/services/site.service';
import { CustomerService } from '../../core/services/customer.service';

@Component({
  selector: 'app-sites',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './sites.component.html',
  styleUrl: './sites.component.scss',
})
export class SitesComponent implements OnInit {
  private siteService = inject(SiteService);
  private customerService = inject(CustomerService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  rawSites: SiteListDto[] = [];
  clients: { id: number; name: string }[] = [];
  showDrawer = false;
  submitted = false;
  isEditMode = false;
  selectedSiteId: number | null = null;

  siteForm!: FormGroup;

  // Pagination State
  currentPage = 1;
  pageSize = 6; // Grid looks best with multiples of 2 or 3
  mathMin = Math.min;

  // Sorting State
  sortField = 'siteName';
  sortAsc = true;

  // Filter State
  searchQuery = '';

  ngOnInit() {
    this.initForm();
    this.loadSites();
    this.loadClientsDropdown();
  }

  initForm() {
    this.siteForm = this.fb.group({
      selectedClient: ['', [Validators.required]],
      name: ['', [Validators.required]],
      address: ['', [Validators.required]],
      contact: ['', [Validators.required]],
      access: [''],
      parking: [''],
      keysCode: [''],
      asbestosRequired: [false],
      ramsRequired: [false],
      notes: ['']
    });
  }

  loadSites() {
    this.siteService.getSites(1, 200).subscribe({
      next: (data) => {
        this.rawSites = data;
      },
      error: () => {
        this.toast.error('Failed to load sites from backend');
      }
    });
  }

  loadClientsDropdown() {
    this.customerService.getCustomersDropdown().subscribe({
      next: (data) => {
        this.clients = data;
      },
      error: () => {
        this.toast.error('Failed to load client list for selection');
      }
    });
  }

  get filteredAndSortedSites() {
    let list = this.rawSites.map(s => ({
      id: `ST-${String(s.id).padStart(4, '0')}`,
      rawId: s.id,
      name: s.siteName || 'Unnamed Site',
      client: s.customer?.name || 'Unknown Client',
      address: s.companyName || 'No Address', // backend uses companyName / area for address
      contact: s.primaryContactName || 'No Contact',
      access: s.primaryContactMobile || 'No Phone',
      ramsRequired: true, // compliance features mapped
      asbestosRequired: true,
      siteName: s.siteName || '',
      customerId: s.customer?.id
    }));

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(s =>
        (s.name && s.name.toLowerCase().includes(q)) ||
        (s.client && s.client.toLowerCase().includes(q)) ||
        (s.address && s.address.toLowerCase().includes(q)) ||
        (s.contact && s.contact.toLowerCase().includes(q))
      );
    }

    // 2. Sorting
    list.sort((a, b) => {
      let valA: any = '';
      let valB: any = '';

      if (this.sortField === 'siteName' || this.sortField === 'name') {
        valA = a.name;
        valB = b.name;
      } else if (this.sortField === 'client') {
        valA = a.client;
        valB = b.client;
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

  get sites() {
    const list = this.filteredAndSortedSites;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedSites.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  firstWord(s: string): string {
    return s ? s.split(' ')[0] : '';
  }

  openDrawer() {
    this.isEditMode = false;
    this.selectedSiteId = null;
    this.submitted = false;

    const initialClient = this.clients.length > 0 ? this.clients[0].id : '';
    this.siteForm.reset({
      selectedClient: initialClient,
      asbestosRequired: false,
      ramsRequired: false
    });
    this.showDrawer = true;
  }

  openEditDrawer(s: any) {
    this.isEditMode = true;
    this.selectedSiteId = s.rawId;
    this.submitted = false;

    this.siteService.getSiteById(s.rawId).subscribe({
      next: (detail) => {
        const contact = detail.contacts && detail.contacts.length > 0 ? detail.contacts[0] : null;
        this.siteForm.patchValue({
          selectedClient: detail.customerId,
          name: detail.siteName,
          address: detail.companyName, // address mapped to companyName field
          contact: contact ? `${contact.firstName} ${contact.lastName}`.trim() : '',
          access: detail.accessDetails,
          parking: detail.parkingInformation,
          keysCode: detail.keysOrCode,
          asbestosRequired: true,
          ramsRequired: true,
          notes: detail.siteNotes
        });
        this.showDrawer = true;
      },
      error: () => {
        this.toast.error('Failed to load site details');
      }
    });
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.siteForm.reset();
  }

  saveSite() {
    this.submitted = true;
    if (this.siteForm.invalid) {
      this.siteForm.markAllAsTouched();
      return;
    }

    const val = this.siteForm.value;
    const nameParts = (val.contact || '').trim().split(/\s+/);
    const firstName = nameParts[0] || '';
    const lastName = nameParts.slice(1).join(' ') || '';

    const selectedClientObj = this.clients.find(c => c.id === Number(val.selectedClient));
    const clientName = selectedClientObj ? selectedClientObj.name : '';

    const payload = {
      customerId: Number(val.selectedClient),
      siteName: val.name,
      companyName: val.address, // save address to companyName field
      area: val.address || '',
      city: 'London',
      countryOrState: 'UK',
      postCode: 'EC1A',
      mobileNumber: '',
      accessDetails: val.access || '',
      parkingInformation: val.parking || '',
      keysOrCode: val.keysCode || '',
      siteNotes: val.notes || '',
      accountManagerId: null,
      contactFirstName: firstName,
      contactLastName: lastName,
      contactMobile: '',
      contactEmail: ''
    };

    if (this.isEditMode && this.selectedSiteId) {
      this.siteService.updateSite(this.selectedSiteId, payload).subscribe({
        next: () => {
          this.toast.success('Site updated successfully');
          this.loadSites();
          this.closeDrawer();
        },
        error: () => {
          this.toast.error('Failed to update site');
        }
      });
    } else {
      this.siteService.createSite(payload).subscribe({
        next: () => {
          this.toast.success('Site registered successfully against client');
          this.loadSites();
          this.closeDrawer();
        },
        error: () => {
          this.toast.error('Failed to register site');
        }
      });
    }
  }
}
