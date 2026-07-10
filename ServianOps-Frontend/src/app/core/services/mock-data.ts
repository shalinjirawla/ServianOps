export type JobStatus =
  | 'new'
  | 'scheduled'
  | 'travelling'
  | 'on-site'
  | 'completed'
  | 'quote-required'
  | 'invoiced';

export const statusColors: Record<JobStatus, string> = {
  'new': 'bg-info/15 text-info border-info/30',
  'scheduled': 'bg-primary/10 text-primary border-primary/20',
  'travelling': 'bg-accent/20 text-accent-foreground border-accent/40',
  'on-site': 'bg-warning/20 text-warning-foreground border-warning/40',
  'completed': 'bg-success/15 text-success border-success/30',
  'quote-required': 'bg-destructive/10 text-destructive border-destructive/30',
  'invoiced': 'bg-muted text-muted-foreground border-border',
};

export const clients = [
  { id: 'CL-001', name: 'Northgate Retail Group', contact: 'Sarah Whitmore', email: 's.whitmore@northgate.co.uk', phone: '+44 20 7946 0821', sites: 14, openJobs: 6, vat: 'GB 234 567 891', terms: 'Net 30' },
  { id: 'CL-002', name: 'Harborline Hospitality', contact: "James O'Connell", email: 'james@harborline.com', phone: '+44 20 7946 3312', sites: 8, openJobs: 3, vat: 'GB 987 654 321', terms: 'Net 14' },
  { id: 'CL-003', name: 'Meridian Facilities Ltd', contact: 'Priya Kapoor', email: 'p.kapoor@meridianfm.co.uk', phone: '+44 20 3941 8820', sites: 22, openJobs: 11, vat: 'GB 445 221 776', terms: 'Net 45' },
  { id: 'CL-004', name: 'Cavendish Property Trust', contact: 'Marcus Ainsley', email: 'm.ainsley@cavendishpt.co.uk', phone: '+44 20 7112 5540', sites: 5, openJobs: 2, vat: 'GB 118 907 442', terms: 'Net 30' },
  { id: 'CL-005', name: 'Blackwood Estates', contact: 'Elena Vasquez', email: 'elena@blackwood-est.com', phone: '+44 20 4560 2233', sites: 9, openJobs: 4, vat: 'GB 552 118 334', terms: 'Net 30' },
];

export const sites = [
  { id: 'ST-1042', client: 'Northgate Retail Group', name: 'Northgate — Canary Wharf', address: '27 Bank St, London E14', contact: 'Ravi Desai', access: 'Loading bay B, 08:00–17:00', ramsRequired: true, asbestosRequired: true },
  { id: 'ST-1055', client: 'Harborline Hospitality', name: 'Harbor Kitchen — Shoreditch', address: '14 Rivington St, London EC2', contact: 'Kate Miles', access: 'Rear entry, code 4471', ramsRequired: true, asbestosRequired: true },
  { id: 'ST-1088', client: 'Meridian Facilities Ltd', name: 'Meridian HQ — Reading', address: 'Forbury Sq, Reading RG1', contact: 'Tom Blake', access: 'Reception check-in', ramsRequired: true, asbestosRequired: true },
  { id: 'ST-1102', client: 'Cavendish Property Trust', name: 'Cavendish House', address: '88 Baker St, London W1', contact: 'Nina Brooks', access: 'Concierge desk', ramsRequired: true, asbestosRequired: true },
  { id: 'ST-1131', client: 'Blackwood Estates', name: 'Blackwood Apartments B', address: '9 Chapel Rd, Bristol BS1', contact: 'David Rhodes', access: 'Keys from lobby', ramsRequired: true, asbestosRequired: true },
];

export interface Job {
  id: string; title: string; client: string; site: string; trade: string;
  priority: 'P1' | 'P2' | 'P3' | 'P4'; status: JobStatus; engineer: string; scheduled: string; po: string;
}

export const jobs: Job[] = [
  { id: 'JB-20481', title: 'Rooftop AC unit not cooling', client: 'Northgate Retail Group', site: 'Northgate — Canary Wharf', trade: 'HVAC', priority: 'P1', status: 'on-site', engineer: 'Owen Hart', scheduled: 'Today, 09:30', po: 'PO-77812' },
  { id: 'JB-20492', title: 'Broken glass door — main entrance', client: 'Harborline Hospitality', site: 'Harbor Kitchen — Shoreditch', trade: 'Glazing', priority: 'P1', status: 'travelling', engineer: 'Amelia Reed', scheduled: 'Today, 11:00', po: 'PO-77820' },
  { id: 'JB-20507', title: 'Quarterly emergency lighting test', client: 'Meridian Facilities Ltd', site: 'Meridian HQ — Reading', trade: 'Electrical', priority: 'P3', status: 'scheduled', engineer: 'Jordan Ellis', scheduled: 'Tomorrow, 08:00', po: 'PO-77835' },
  { id: 'JB-20518', title: "Leaking urinal — men's WC 2F", client: 'Cavendish Property Trust', site: 'Cavendish House', trade: 'Plumbing', priority: 'P2', status: 'completed', engineer: 'Owen Hart', scheduled: 'Yesterday, 14:00', po: 'PO-77841' },
  { id: 'JB-20522', title: 'Faulty access control reader', client: 'Blackwood Estates', site: 'Blackwood Apartments B', trade: 'Security', priority: 'P2', status: 'quote-required', engineer: 'Amelia Reed', scheduled: 'Wed, 10:00', po: 'PO required' },
  { id: 'JB-20530', title: 'Boiler pressure loss investigation', client: 'Northgate Retail Group', site: 'Northgate — Canary Wharf', trade: 'Heating', priority: 'P2', status: 'new', engineer: '—', scheduled: 'Unassigned', po: 'PO-77860' },
  { id: 'JB-20544', title: 'Kitchen extract deep clean', client: 'Harborline Hospitality', site: 'Harbor Kitchen — Shoreditch', trade: 'Cleaning', priority: 'P4', status: 'invoiced', engineer: 'Jordan Ellis', scheduled: 'Last week', po: 'PO-77790' },
];

export const engineers = [
  { id: 'EN-01', name: 'Owen Hart', trade: 'HVAC / Plumbing', availability: 'On Job', jobs: 3, rating: 4.8, region: 'London Central' },
  { id: 'EN-02', name: 'Amelia Reed', trade: 'Glazing / Security', availability: 'Travelling', jobs: 2, rating: 4.9, region: 'London East' },
  { id: 'EN-03', name: 'Jordan Ellis', trade: 'Electrical', availability: 'Available', jobs: 4, rating: 4.7, region: 'Home Counties' },
  { id: 'EN-04', name: 'Priya Shah', trade: 'Fabric / Carpentry', availability: 'Off shift', jobs: 0, rating: 4.6, region: 'London South' },
  { id: 'EN-05', name: 'Marcus Doyle', trade: 'Refrigeration', availability: 'Available', jobs: 1, rating: 4.8, region: 'London West' },
];

export const quotes = [
  { id: 'QT-3401', job: 'JB-20522', client: 'Blackwood Estates', scope: 'Replace 2× access control readers + PSU', value: 1840, status: 'sent', created: '2 days ago' },
  { id: 'QT-3402', job: 'JB-19980', client: 'Meridian Facilities Ltd', scope: 'Re-wire 1F distribution board', value: 6420, status: 'approved', created: '5 days ago' },
  { id: 'QT-3403', job: 'JB-19844', client: 'Cavendish Property Trust', scope: 'Replace 4× radiator TRVs', value: 620, status: 'declined', created: '1 week ago' },
  { id: 'QT-3404', job: 'JB-20488', client: 'Northgate Retail Group', scope: 'Refurbish rooftop condenser coils', value: 3980, status: 'sent', created: 'yesterday' },
];

export const invoices = [
  { id: 'INV-8821', client: 'Cavendish Property Trust', job: 'JB-20518', amount: 340, status: 'paid', issued: '3 days ago' },
  { id: 'INV-8822', client: 'Harborline Hospitality', job: 'JB-20544', amount: 780, status: 'sent', issued: '1 day ago' },
  { id: 'INV-8823', client: 'Meridian Facilities Ltd', job: 'JB-19980', amount: 6420, status: 'overdue', issued: '35 days ago' },
  { id: 'INV-8824', client: 'Northgate Retail Group', job: 'JB-20481', amount: 1240, status: 'draft', issued: '—' },
];

export const activityFeed = [
  { t: '3m ago', who: 'Owen Hart', what: 'arrived on site', ref: 'JB-20481' },
  { t: '18m ago', who: 'Amelia Reed', what: 'accepted job', ref: 'JB-20492' },
  { t: '42m ago', who: 'Planner', what: 'assigned job to Jordan Ellis', ref: 'JB-20507' },
  { t: '1h ago', who: 'Owen Hart', what: 'marked job complete', ref: 'JB-20518' },
  { t: '2h ago', who: 'System', what: 'raised follow-up quote', ref: 'QT-3401' },
  { t: '5h ago', who: 'Sarah Whitmore', what: 'approved PO', ref: 'PO-77860' },
];