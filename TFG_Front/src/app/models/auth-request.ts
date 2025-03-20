export interface AuthRequest {
    UserEmail?: string;
    UserNickname?: string;
    UsuarioEmailOApodo?: string;
    UserPassword: string;
    UserEmailOrNickname?: string;
    UserProfilePhoto?: string;
    Remember?:boolean;
  }