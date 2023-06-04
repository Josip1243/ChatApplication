export class Message {
    id!: number;
    userId!: number;
    username!: string;
    text!: string;
    timestamp!: Date;
}

export class User {
    id!: number;
    username!: string;
}