// Define la estructura para crear o actualizar una imagen.
export interface CreateOrUpdateImageRequest {
    name: string;
    file: File;
}