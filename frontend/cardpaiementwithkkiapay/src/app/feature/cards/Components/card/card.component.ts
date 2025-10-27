import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css'],
})
export class CardComponent implements OnInit, OnDestroy {
  currentUser: any = null;
  selectedCard: string | null = null;
  private authSubscription?: Subscription;

  constructor(private router: Router, private authService: AuthService) {}

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
    } else {
      this.selectedCard = cardType;
    }
  }

  processPayment(): void {
    if (!this.selectedCard) {
      return;
    }

    switch (this.selectedCard) {
      case 'success':
        break;
      case 'insufficient':
        break;
      case 'declined':
        break;
      case 'fraudulent':
        break;
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
