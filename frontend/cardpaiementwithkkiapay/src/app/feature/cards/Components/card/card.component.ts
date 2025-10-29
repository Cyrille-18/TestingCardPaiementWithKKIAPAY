import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../../../shared/navbar/navbar.component';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { Subscription } from 'rxjs';
import {
  CardSelectionService,
  CardDetails,
} from '../../../../shared/services/card-selection.service';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css'],
})
export class CardComponent implements OnInit, OnDestroy {
  currentUser: any = null;
  selectedCard: string | null = null;
  private authSubscription?: Subscription;

  constructor(
    private router: Router,
    private authService: AuthService,
    private cardSelectionService: CardSelectionService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();

    this.authSubscription = this.authService.isAuthenticated$.subscribe(
      (isAuthenticated) => {
        if (!isAuthenticated) {
          this.currentUser = null;
          this.router.navigate(['/login']);
        } else {
          this.currentUser = this.authService.getCurrentUser();
        }
      }
    );
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  selectCard(cardType: string): void {
    if (this.selectedCard === cardType) {
      this.selectedCard = null;
      this.cardSelectionService.clearSelection();
    } else {
      this.selectedCard = cardType;

      // Store card details based on type
      const cardDetails: CardDetails = this.getCardDetails(cardType);
      this.cardSelectionService.setSelectedCard(cardDetails);
    }
  }

  private getCardDetails(cardType: string): CardDetails {
    const cardMap: { [key: string]: CardDetails } = {
      success: {
        type: 'success',
        number: '4556 0527 0417 2643',
        cvv: '3310',
        expiry: '09/32',
        pin: '899',
        otp: '12345',
      },
      insufficient: {
        type: 'insufficient',
        number: '5258 5859 2266 6506',
        cvv: '3310',
        expiry: '09/31',
        pin: '883',
        otp: '12345',
      },
      declined: {
        type: 'declined',
        number: '5143 0105 2233 9965',
        cvv: '3310',
        expiry: '08/32',
        pin: '276',
        otp: '-----',
      },
      fraudulent: {
        type: 'fraudulent',
        number: '5143 0105 2233 9965',
        cvv: '3310',
        expiry: '08/32',
        pin: '276',
        otp: '12345',
      },
    };

    return cardMap[cardType] || cardMap['success'];
  }

  processPayment(): void {
    if (!this.selectedCard) {
      return;
    }

    this.router.navigate(['/product']);
  }

  logout(): void {
    this.authService.logout();
  }
}
