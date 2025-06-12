// Define la estructura de un usuario en el frontend.
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
    // Propiedad opcional para controlar el estado de edición en la UI.
    editing?: boolean; 
}