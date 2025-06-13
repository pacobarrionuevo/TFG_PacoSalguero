// Define la estructura de los datos para una solicitud de autenticación (login/registro).
export interface AuthRequest {
    UserEmail?: string;
    UserNickname?: string;
    UserPassword: string;
    // Campo unificado para enviar email o nickname en el login
    UserEmailOrNickname?: string;
    UserProfilePhoto?: string;
    Remember?:boolean;
  }