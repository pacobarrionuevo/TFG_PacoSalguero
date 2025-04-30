export interface AuthRequest {
    UserEmail?: string;
    UserNickname?: string;
    UserPassword: string;
    UserEmailOrNickname?: string;
    UserProfilePhoto?: string;
    Remember?:boolean;
  }