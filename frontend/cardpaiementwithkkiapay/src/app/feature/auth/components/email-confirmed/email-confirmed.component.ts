import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-email-confirmed',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './email-confirmed.component.html',
  styleUrl: './email-confirmed.component.css',
})
export class EmailConfirmedComponent implements OnInit, OnDestroy {
  countdown: number = 5;
  private intervalId: any;
  isConfirmed: boolean = false;
  isLoading: boolean = true;
  errorMessage: string = '';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.confirmEmail();
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  private confirmEmail(): void {
    const userId = this.route.snapshot.queryParams['userId'];
    const token = this.route.snapshot.queryParams['token'];

    if (!userId || !token) {
      this.errorMessage = 'Paramètres de confirmation manquants';
      this.isLoading = false;
      return;
    }

    // Appel au service pour confirmer l'email
    this.authService.confirmEmail(userId, token).subscribe({
      next: (response) => {
        this.isConfirmed = true;
        this.isLoading = false;
        this.startCountdown();
      },
      error: (error) => {
        this.isConfirmed = false;
        this.isLoading = false;
        this.errorMessage = "Erreur lors de la confirmation de l'email";
        console.error('Erreur confirmation email:', error);
      },
    });
  }

  private startCountdown(): void {
    this.intervalId = setInterval(() => {
      this.countdown--;
      if (this.countdown <= 0) {
        clearInterval(this.intervalId);
        this.goToLogin();
      }
    }, 1000);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
