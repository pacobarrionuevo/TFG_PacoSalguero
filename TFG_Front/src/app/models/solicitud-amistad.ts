// Define la estructura de una solicitud de amistad pendiente.
export interface SolicitudAmistad {
    friendshipId: number;
    userId: number;
    userNickname: string;
    userProfilePhoto?: string;
}