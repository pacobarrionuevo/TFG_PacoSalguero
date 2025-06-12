# TFG Sanitarios - Gestión Integral para Profesionales

Trabajo de Fin de Grado de los alumnos Paco Barrionuevo y Jorge Salguero del 2º año del CPIFP Alan Turing en Desarrollo de Aplicaciones Multiplataforma.

- **Enlace al anteproyecto:** [Documento de Google Docs](https://docs.google.com/document/d/1TFJnD-q_kEXWpyvHYLzhxsQBzPGWbPmFwfnIWQeBnkk/edit?usp=sharing)
- **Enlace a los diseños en Figma:** *(aquí podéis añadir el enlace)* (Aún hay vistas por añadir).

---

## 1. Introducción

**Sanitarios App** es una solución integral diseñada para facilitar la gestión diaria de los profesionales del sector sanitario. El proyecto consiste en una plataforma completa que incluye una aplicación web, una aplicación móvil nativa para Android y un panel de informes de Business Intelligence.

El objetivo principal es proporcionar a los sanitarios una herramienta centralizada para organizar su agenda profesional, gestionar la información de sus clientes y servicios, administrar la facturación y mantener una comunicación fluida con su red de contactos.

## 2. Arquitectura y Tecnologías

El proyecto se estructura en cuatro componentes principales, cada uno desarrollado con tecnologías específicas para optimizar su rendimiento y funcionalidad:

-   **Backend (TFG_Back)**: Desarrollado con **ASP.NET Core 8**, sigue una arquitectura por capas utilizando el patrón Repositorio y Unit of Work para una gestión de datos robusta y escalable.
    -   **Base de datos**: SQLite gestionada a través de Entity Framework Core.
    -   **Autenticación**: Sistema basado en JSON Web Tokens (JWT) para proteger los endpoints.
    -   **Tiempo Real**: Implementación de **WebSockets** para notificaciones instantáneas, como el estado de conexión de los usuarios y solicitudes de amistad.
    -   **API**: Una API RESTful que sirve como nexo de unión entre el backend y los clientes (web y móvil).

-   **Frontend (TFG_Front)**: Una Single Page Application (SPA) creada con **Angular 18**.
    -   **Diseño**: Componentes estructurados por páginas y un layout principal con menú de navegación para una experiencia de usuario coherente.
    -   **Comunicación**: Servicios dedicados para consumir la API del backend y un servicio de WebSocket para la interacción en tiempo real.
    -   **Funcionalidades**: Panel de administración, gestión de agenda, calendario interactivo, sistema de amigos, ficheros y facturación.

-   **Aplicación Móvil (TFG_Movil)**: App nativa para **Android** desarrollada en **Kotlin** y **Jetpack Compose**.
    -   **Arquitectura**: Sigue el patrón MVVM (Model-View-ViewModel) para una separación clara de responsabilidades.
    -   **Comunicación**: Utiliza **Retrofit** para las llamadas a la API y **DataStore** para la persistencia local de credenciales de usuario.
    -   **Enfoque**: Ofrece las funcionalidades esenciales de autenticación y navegación, proporcionando una experiencia optimizada para dispositivos móviles.

-   **Business Intelligence (Informe Power BI.pbix)**: Un informe interactivo creado con **Power BI** que se conecta a la base de datos de la aplicación para visualizar métricas clave y facilitar la toma de decisiones basada en datos.

## 3. Paleta de Colores y Diseño

La selección de colores de la aplicación se ha realizado con el objetivo de transmitir profesionalidad, confianza y calma, valores fundamentales en el sector sanitario.

-   **Azul (`#0077c2`, `#4498D0`, `#DCE8F3`)**: Es el color predominante. Psicológicamente, el azul se asocia con la serenidad, la estabilidad y la confianza. Se utiliza en elementos principales, fondos y enlaces para crear un entorno visual limpio y profesional.
-   **Púrpura/Morado (`#2a0066`, `#5f00b2`)**: Utilizado en el menú de navegación, este color aporta un toque de modernidad y sofisticación. Al ser un color menos común, ayuda a diferenciar claramente el área de navegación del contenido principal.
-   **Naranja/Amarillo (`#ff9900`, `#ffcc00`)**: Empleado en botones de acción importantes como "Iniciar Sesión" o "Registrarse". Estos colores cálidos y enérgicos captan la atención del usuario y lo guían hacia acciones clave.
-   **Verde (`#28a745`)**: Se usa para indicar acciones exitosas (botones de "Guardar") y estados positivos ("Conectado"), ya que universalmente se asocia con lo correcto y la seguridad.
-   **Rojo (`#dc3545`)**: Reservado para acciones destructivas o de advertencia, como los botones de "Eliminar" o "Rechazar", alertando al usuario sobre la importancia de su decisión.

Esta combinación de colores no solo busca un resultado estético agradable, sino que también mejora la usabilidad de la interfaz al crear una jerarquía visual clara y guiar al usuario de forma intuitiva.

## 4. Modelo Entidad-Relación

La base de datos es el núcleo de la aplicación. Su diseño se ha estructurado para garantizar la integridad de los datos y la eficiencia en las consultas. A continuación se muestra el diagrama Entidad-Relación:


![Diagrama Entidad-Relación](URL_DE_LA_IMAGEN)
#### Entidades Principales:

-   **User**: Almacena la información de los usuarios, incluyendo credenciales para la autenticación y su rol en el sistema (usuario o administrador).
-   **Customer**: Contiene los datos de los clientes del profesional sanitario. Se relaciona con `PaymentMethod`.
-   **EntradaAgenda**: Representa cada cita o evento en la agenda del usuario. Se vincula con un `Service` para detallar el tipo de servicio prestado.
-   **Service**: Define los diferentes tipos de servicios que ofrece el profesional, cada uno con su nombre, abreviatura y un color para una fácil identificación visual en la agenda.
-   **Friendship / UserHasFriendship**: Implementan un sistema de amistad (relación N:M entre usuarios), permitiendo a los usuarios conectar entre sí y ver su estado de conexión en tiempo real.
-   **ServiceFacturado**: Una tabla para registrar los servicios que ya han sido procesados para su facturación.

## 5. Funcionalidades Implementadas y Principales

La aplicación ofrece un conjunto de herramientas diseñadas para cubrir las necesidades clave de un profesional sanitario:

-   **Gestión de Usuarios y Autenticación**:
    -   Registro e inicio de sesión seguros mediante JWT.
    -   Sistema de roles (Usuario y Administrador) para controlar el acceso a las diferentes funcionalidades.
    -   Opción de "Recuérdame" para una experiencia de usuario más fluida.

-   **Panel de Administración**:
    -   Dashboard con estadísticas clave (total de usuarios, clientes, servicios, etc.).
    -   Gestión completa de usuarios: editar datos, cambiar roles, modificar avatares y eliminar usuarios.

-   **Módulo de Agenda y Calendario**:
    -   Creación de entradas en la agenda con información detallada: cliente, centro, servicio, precio y observaciones.
    -   Visualización de la agenda en formato de lista cronológica y en un calendario mensual interactivo.
    -   Uso de colores para diferenciar los tipos de servicio de un vistazo.

-   **Gestión de Ficheros**:
    -   CRUD completo para **Servicios**, **Métodos de Pago** y **Clientes**, permitiendo una gestión centralizada de la información maestra de la aplicación.

-   **Módulo Social y Comunicación en Tiempo Real**:
    -   Sistema de amigos: enviar, aceptar y rechazar solicitudes.
    -   Lista de amigos con indicador de estado (conectado/desconectado) y última hora de conexión, actualizado en tiempo real mediante WebSockets.
    -   Buscador de usuarios para añadir nuevos contactos.

-   **Facturación y Reportes**:
    -   Selección de entradas de la agenda para agruparlas y facturarlas.
    -   (En desarrollo) Generación de facturas en formato PDF a partir de los servicios seleccionados.
    -   Integración con un informe de Power BI para el análisis de datos de la aplicación.

## 📌 Estado Actual del Proyecto

-   La estructura general del sistema está implementada y las funcionalidades base de autenticación, gestión de datos y navegación están operativas.
-   La aplicación web y el servidor están en una fase avanzada, con la mayoría de las funcionalidades principales implementadas.
-   El despliegue de la web está en fase final de preparación.
-   La aplicación móvil cuenta con la configuración inicial y las funcionalidades básicas de autenticación, siendo el siguiente foco de desarrollo.
-   Debemos terminar de implementar el funcionamiento básico de lo que debe hacer la aplicación. En el siguiente vídeo se podrá ver lo que llevamos hasta ahora.
