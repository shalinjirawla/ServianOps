export interface LoginDto {
  email: string;
  password?: string;
  tenancyName?: string;
}

export interface AuthResponseDto {
  userId: number;
  tenantId: number | null;
  email: string;
  role: string;
}

export interface UserSession {
  userId: number;
  tenantId: number | null;
  email: string;
  role: string;
}
