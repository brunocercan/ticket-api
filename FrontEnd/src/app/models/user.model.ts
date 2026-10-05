export interface User {
  idUsuario: number;
  nome: string;
  emailUsuario: string;
  funcao: string;
  dataCriacao?: Date | null;
}

export interface CreateUserRequest {
  nome: string;
  email: string;
  senha: string;
  funcao: string;
}

export interface UpdateUserRequest {
  id: number;
  nome: string;
  email: string;
  funcao: string;
  dataCriacao: Date;
  senha?: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
}