export interface LoginDto {
  email: string;
  password?: string;
  tenancyName?: string;
}

export interface AuthResponseDto {
  token: string;
  userId: number;
  tenantId: number | null;
  email: string;
  role: string;
}

export interface DecodedToken {
  sub: string;
  email: string;
  jti: string;
  tenant_id: string | null;
  user_id: string;
  role: string;
  exp: number; // Unix timestamp
  iss?: string;
  aud?: string;
}

export interface UserSession {
  token: string;
  userId: number;
  tenantId: number | null;
  email: string;
  role: string;
  decodedToken: DecodedToken;
}
