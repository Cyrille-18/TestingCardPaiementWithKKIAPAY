import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../auth/services/auth.service';
import { Subscription } from 'rxjs';
import { NavbarComponent } from '../../shared/navbar/navbar.component';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent implements OnInit, OnDestroy {
  currentUser: any = null;
  private authSubscription?: Subscription;
  selectedOption: 'buy' | 'subscribe' | null = null;

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

  selectOption(option: 'buy' | 'subscribe'): void {
    this.selectedOption = this.selectedOption === option ? null : option;
  }

  continue(): void {
    if (!this.selectedOption) {
      return;
    }
    if (this.selectedOption === 'buy') {
      this.router.navigate(['/purchase']);
    } else if (this.selectedOption === 'subscribe') {
      this.router.navigate(['/subscription']);
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
