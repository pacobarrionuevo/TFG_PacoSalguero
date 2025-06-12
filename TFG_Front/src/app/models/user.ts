export interface User {
    userId?: number;
    userNickname?: string;
    userEmail?: string;
    userProfilePhoto?: string; 
    userPassword?: string; 
    userConfirmPassword?: string;
    role?: 'user' | 'admin';
    isOnline?: boolean;
    lastSeen?: string;
    esAmigo?: boolean;
}