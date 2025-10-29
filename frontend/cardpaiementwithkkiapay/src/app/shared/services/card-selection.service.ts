import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface CardDetails {
  type: string;
  number: string;
  cvv: string;
  expiry: string;
  pin: string;
  otp: string;
}

@Injectable({
  providedIn: 'root',
})
export class CardSelectionService {
  private selectedCardSubject = new BehaviorSubject<CardDetails | null>(null);
  public selectedCard$ = this.selectedCardSubject.asObservable();

  setSelectedCard(card: CardDetails): void {
    this.selectedCardSubject.next(card);
  }

  getSelectedCard(): CardDetails | null {
    return this.selectedCardSubject.value;
  }

  clearSelection(): void {
    this.selectedCardSubject.next(null);
  }
}
