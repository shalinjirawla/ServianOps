import { Injectable, signal } from '@angular/core';
import { clients as mockClients, sites as mockSites } from './mock-data';

export interface Client {
  id: string;
  name: string;
  contact: string;
  email: string;
  phone: string;
  sites: number;
  openJobs: number;
  vat: string;
  terms: string;
  billingAddress?: string;
  billingEmail?: string;
  hourlyRate?: number;
  calloutFee?: number;
  outOfHoursMultiplier?: number;
  slaP1?: number;
  slaP2?: number;
  slaP3?: number;
  slaP4?: number;
  poRequired?: boolean;
  poLimit?: number;
  emailInvoice?: boolean;
  autoNotify?: boolean;
}

export interface Site {
  id: string;
  client: string;
  name: string;
  address: string;
  contact: string;
  access: string;
  parking?: string;
  keysCode?: string;
  asbestosRequired?: boolean;
  ramsRequired?: boolean;
  notes?: string;
}

export interface Tenant {
  id: string;
  companyName: string;
  tenancyName: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  planId: number | null;
}

@Injectable({ providedIn: 'root' })
export class DataService {
  private CLIENTS_KEY = 'servianOps.clients';
  private SITES_KEY = 'servianOps.sites';
  private TENANTS_KEY = 'servianOps.tenants';

  clients = signal<Client[]>([]);
  sites = signal<Site[]>([]);
  tenants = signal<Tenant[]>([]);

  private mockTenants: Tenant[] = [
    { id: 'TN-001', companyName: 'Apex Logistics', tenancyName: 'apex', firstName: 'Sarah', lastName: 'Connor', email: 's.connor@apex.com', phone: '+44 7911 123456', planId: 2 },
    { id: 'TN-002', companyName: 'Zenith Holdings', tenancyName: 'zenith', firstName: 'David', lastName: 'Gandy', email: 'd.gandy@zenith.com', phone: '+44 7911 234567', planId: 3 },
  ];

  constructor() {
    this.loadData();
  }

  private loadData() {
    const cachedClients = localStorage.getItem(this.CLIENTS_KEY);
    if (cachedClients) {
      this.clients.set(JSON.parse(cachedClients));
    } else {
      this.clients.set(mockClients);
      this.saveClients();
    }

    const cachedSites = localStorage.getItem(this.SITES_KEY);
    if (cachedSites) {
      this.sites.set(JSON.parse(cachedSites));
    } else {
      this.sites.set(mockSites);
      this.saveSites();
    }

    const cachedTenants = localStorage.getItem(this.TENANTS_KEY);
    if (cachedTenants) {
      this.tenants.set(JSON.parse(cachedTenants));
    } else {
      this.tenants.set(this.mockTenants);
      this.saveTenants();
    }
  }

  saveClients() {
    localStorage.setItem(this.CLIENTS_KEY, JSON.stringify(this.clients()));
  }

  saveSites() {
    localStorage.setItem(this.SITES_KEY, JSON.stringify(this.sites()));
  }

  saveTenants() {
    localStorage.setItem(this.TENANTS_KEY, JSON.stringify(this.tenants()));
  }

  addClient(c: Omit<Client, 'id' | 'sites' | 'openJobs'>) {
    const nextId = `CL-${String(this.clients().length + 1).padStart(3, '0')}`;
    const newClient: Client = {
      ...c,
      id: nextId,
      sites: 0,
      openJobs: 0,
    };
    this.clients.update((list) => [...list, newClient]);
    this.saveClients();
    return newClient;
  }

  addSite(s: Omit<Site, 'id'>) {
    const nextId = `ST-${String(this.sites().length + 1001)}`;
    const newSite: Site = {
      ...s,
      id: nextId,
    };
    this.sites.update((list) => [...list, newSite]);
    this.saveSites();

    // Increment client site count
    this.clients.update((list) =>
      list.map((c) => (c.name === s.client ? { ...c, sites: c.sites + 1 } : c))
    );
    this.saveClients();

    return newSite;
  }

  addTenant(t: Omit<Tenant, 'id'>) {
    const nextId = `TN-${String(this.tenants().length + 1).padStart(3, '0')}`;
    const newTenant: Tenant = {
      ...t,
      id: nextId,
    };
    this.tenants.update((list) => [...list, newTenant]);
    this.saveTenants();
    return newTenant;
  }

  updateTenant(id: string, updated: Partial<Tenant>) {
    this.tenants.update((list) =>
      list.map((t) => (t.id === id ? { ...t, ...updated } : t))
    );
    this.saveTenants();
  }

  deleteTenant(id: string) {
    this.tenants.update((list) => list.filter((t) => t.id !== id));
    this.saveTenants();
  }
}
