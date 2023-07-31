export class Message {
  Id!: number;
  userId!: number;
  username!: string;
  chatId!: number;
  content!: string;
  sentAt!: Date;
}

export class User {
  id!: number;
  username!: string;
}

export class ChatNameDTO {
  id!: number;
  name!: string;
  deleted!: boolean;
}

export class ChatDTO {
  id!: number;
  name!: string;
  messages!: Message[];
}
