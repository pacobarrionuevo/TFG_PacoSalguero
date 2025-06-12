// Define la estructura de la respuesta al enviar una solicitud de amistad.
export interface SendFriendRequest {
    success: boolean;
    amistadId: number;
    senderName: string;
}