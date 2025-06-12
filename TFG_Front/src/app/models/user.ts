export interface User {
    UserId?: number;
    UserNickname?: string;
    UserEmail?: string;
    UserProfilePhoto?: string; 
    UserPassword?: string; 
    UserConfirmPassword?: string;
    Role?: 'user' | 'admin';
    isOnline?: boolean;
    lastSeen?: string;
    EsAmigo?: boolean;
}