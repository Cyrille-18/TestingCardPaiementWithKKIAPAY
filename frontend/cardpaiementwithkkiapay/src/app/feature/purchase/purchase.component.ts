import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import { Router } from '@angular/router';
import { AuthService } from '../auth/services/auth.service';
import { Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CardSelectionService } from '../../shared/services/card-selection.service';

@Component({
  selector: 'app-purchase',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css'],
})
export class PurchaseComponent implements OnInit, OnDestroy {
  currentUser: any = null;
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

  back(): void {
    this.router.navigate(['/product']);
  }

  proceedToPayment(): void {
    const selectedCard = this.cardSelectionService.getSelectedCard();

    const openWidget = (window as any)?.openKkiapayWidget;
    if (typeof openWidget === 'function') {
      openWidget({
        amount: 49900,
        api_key: environment.publickkiapaykey,
        sandbox: true,
        currency: 'XOF',
        email: this.currentUser?.email ?? undefined,
        paymentmethod: 'card', // Force card payment only
        // Add metadata about selected card type for backend processing
        ...(selectedCard && {
          metadata: {
            card_type: selectedCard.type,
            test_scenario: selectedCard.type,
          },
        }),
      });

      const addSuccess = (window as any)?.addSuccessListener;
      const addFailure = (window as any)?.addFailureListener;
      if (typeof addSuccess === 'function') {
        addSuccess((response: any) => {
          console.log('Payment success:', response);
          console.log('Selected card type:', selectedCard?.type);
          // Handle success - maybe navigate to success page
        });
      }
      if (typeof addFailure === 'function') {
        addFailure((err: any) => {
          console.log('Payment failed:', err);
          console.log('Selected card type:', selectedCard?.type);
          // Handle failure
        });
      }
    } else {
      this.router.navigate(['/card']);
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
