import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { ChatNameDTO, ChatDTO } from 'src/app/shared/models/chat.models';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  baseUrl = 'http://localhost:5220/';
  private currentChatId = new BehaviorSubject<number>(1);
  currentChat = this.currentChatId.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  public getAllChats(): Observable<ChatNameDTO[]> {
    return this.http.get<ChatNameDTO[]>(this.baseUrl + 'api/chat/getAllChats');
  }

  public getChat(chatId: number): Observable<ChatDTO> {
    let params = new HttpParams();
    params = params.append('chatId', chatId);
    return this.http.get<ChatDTO>(this.baseUrl + 'api/chat/getChat', {
      params: params,
    });
  }

  public changeChat(chatId: number) {
    this.currentChatId.next(chatId);
  }

  public addChat(username: string) {
    this.http.post<ChatDTO>(this.baseUrl + 'api/chat/addChat', username);
  }

  public removeChat(chatId: number) {
    return this.http.delete(this.baseUrl + 'api/chat/removeChat/' + chatId);
  }
}
