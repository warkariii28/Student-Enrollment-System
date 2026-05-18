export interface AdminAuditLog {
  auditLogID: number;
  adminUserID: number;
  adminName: string;
  adminEmail: string;
  action: string;
  entityName: string;
  entityID: number | null;
  details: string | null;
  ipAddress: string | null;
  createdAt: string;
}
