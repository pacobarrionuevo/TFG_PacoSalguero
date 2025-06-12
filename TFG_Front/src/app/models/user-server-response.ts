// Define una estructura de respuesta de usuario del servidor (posiblemente obsoleta o para un uso específico).
export interface UsuarioServerResponse {
    usuarioId?: number;
    usuarioApodo?: string;
    usuarioEmail?: string;
    usuarioFotoPerfil?: string;
    usuarioEstado?: string;
    rol?: 'usuario' | 'admin';
    estaBaneado?: boolean;
    esAmigo?: boolean;
  }