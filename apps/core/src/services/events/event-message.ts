export interface EventMessage<T>
{
    id: string;
    data: T;
    message: string;
}