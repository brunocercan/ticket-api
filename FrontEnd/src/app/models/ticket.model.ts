export interface Ticket {
  id: number;
  titulo: string;
  descricao: string;
  prioridade: string;
  status: string;
  idCategoria: number;
  idSolicitante: number;
  idVinculado?: number | null;
  dataCriacao: Date;
  dataAtualizacao?: Date | null;
  dataFechamento?: Date | null;
}

export interface TicketDetail extends Ticket {
  nomeCategoria: string;
  nomeSolicitante: string;
  nomeResponsavelChamado?: string;
  detalhesTicket?: TicketComment[];
}

export interface TicketComment {
  idComentario: number;
  idTicket: number;
  idUsuario: number;
  conteudo: string;
  dataCriacao: Date;
}

export interface CreateTicketRequest {
  titulo: string;
  descricao: string;
  prioridade: string;
  status: string;
  idCategoria: number;
  idSolicitante: number;
  idVinculado?: number | null;
}

export interface UpdateTicketRequest {
  id: number;
  titulo: string;
  descricao: string;
  prioridade: string;
  status: string;
  idCategoria: number;
  idSolicitante: number;
  idVinculado: number;
  dataCriacao: Date;
  dataAlteracao: Date;
  dataFechamento: Date;
}

export interface TicketFilters {
  id?: number | null;
  titulo?: string | null;
  prioridade?: string | null;
  status?: string | null;
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateCommentRequest {
  ticketId: number;
  userId: number;
  content: string;
}

export interface PagedResponse<T> {
  data: T[];
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}