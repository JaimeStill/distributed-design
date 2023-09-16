export interface ApiMessage<T>
{
    data?: T;
    message: string;
    error: boolean;
    hasData: boolean;
}