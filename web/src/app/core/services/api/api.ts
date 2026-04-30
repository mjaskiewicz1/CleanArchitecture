import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { httpResource } from '@angular/common/http';
import { HttpResourceRef } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class Api {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = 'https://localhost:5001/api';

  protected get<T>(path: string): HttpResourceRef<T | undefined> {
    return httpResource<T>(() => `${this.baseUrl}/${path}`);
  }

  protected post<T>(path: string, body: unknown) {
    return this.http.post<T>(`${this.baseUrl}/${path}`, body);
  }

  protected put<T>(path: string, body: unknown) {
    return this.http.put<T>(`${this.baseUrl}/${path}`, body);
  }
}
