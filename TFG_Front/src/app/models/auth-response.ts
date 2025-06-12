// Define la estructura de la respuesta del servidor tras una autenticación exitosa.
export interface AuthResponse {
    stringToken: string;  
    userId: number;   
    isAdmin: boolean;
}