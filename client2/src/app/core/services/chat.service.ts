import { HttpClient, HttpParams } from '@angular/common/http';
import { NONE_TYPE } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, ReplaySubject } from 'rxjs';
import { ChatNameDTO, ChatDTO } from 'src/app/shared/models/chat.models';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  baseUrl = 'http://localhost:5220/';

  private currentChatSubject = new ReplaySubject<ChatDTO | undefined>();
  currentChat = this.currentChatSubject.asObservable();

  private chatsSubject = new BehaviorSubject<ChatNameDTO[]>([]);
  allChats = this.chatsSubject.asObservable();

  constructor(private http: HttpClient) {}

  public refreshChats() {
    this.getAllChats();
  }

  public deleteChat() {
    this.refreshChats();
    this.currentChatSubject.next(undefined);
  }

  public resetChatService() {
    this.currentChatSubject.complete();
    this.chatsSubject.complete();
    this.currentChatSubject = new ReplaySubject<ChatDTO | undefined>();
    this.chatsSubject = new BehaviorSubject<ChatNameDTO[]>([]);
    this.currentChat = this.currentChatSubject.asObservable();
    this.allChats = this.chatsSubject.asObservable();
  }

  public getAllChats() {
    this.http
      .get<ChatNameDTO[]>(this.baseUrl + 'api/chat/getAllChats')
      .subscribe((c) => {
        this.chatsSubject.next(c);
      });
  }

  public getChat(chatId: number): Observable<ChatDTO> {
    let params = new HttpParams();
    params = params.append('chatId', chatId);
    return this.http.get<ChatDTO>(this.baseUrl + 'api/chat/getChat', {
      params: params,
    });
  }

  public changeChat(chatId: number) {
    this.getChat(chatId).subscribe((c) => {
      this.currentChatSubject.next(c);
    });
  }

  public addChat(username: string) {
    let params = new HttpParams();
    params = params.append('username', username);
    return this.http.get<ChatDTO>(this.baseUrl + 'api/chat/addChat', {
      params: params,
    });
  }

  public removeChat(chatId: number) {
    return this.http.delete(this.baseUrl + 'api/chat/removeChat/' + chatId);
  }
}
