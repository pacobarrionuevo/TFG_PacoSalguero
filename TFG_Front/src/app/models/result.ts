// Clase genérica para estandarizar las respuestas de la API.
// Permite un manejo consistente de éxitos y errores en los servicios.
export class Result<T = void> {
    [x: string]: any;
    success: boolean;
    statusCode: number;
    error: string;
    data: T;
  
    private constructor(success: boolean, statusCode: number, error: string = null, data: T = null) {
      this.success = success;
      this.error = error;
      this.statusCode = statusCode;
      this.data = data;
    }
  
    isSuccess(): boolean {
      return this.success;
    }
  
    getData(): T | null {
      return this.data;
    }
  
    getError(): string | null {
      return this.error;
    }
  
    throwIfError() {
      if (!this.success) {
        throw new Error(this.error);
      }
    }
  
    // Método estático para crear un resultado exitoso.
    static success<T = void>(statusCode: number, data: T = null): Result<T> {
      return new Result(true, statusCode, null, data);
    } 
  
    // Método estático para crear un resultado de error.
    static error<T = void>(statusCode: number, error: string = null): Result<T> {
      return new Result(false, statusCode, error);
    } 
  }