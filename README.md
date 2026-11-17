# CardPaiementWithKkiapay

Application de test d’authentification et de paiement par carte avec
[Kkiapay](https://docs.kkiapay.me/). Le projet est composé d’une API ASP.NET
Core et d’une application Angular.

> Le paiement est configuré en mode **sandbox** afin de tester le parcours de
> paiement par carte. Les montants utilisés dans l’interface sont des valeurs
> de test.



## Fonctionnalités

- inscription et confirmation d’adresse e-mail ;
- connexion avec JWT et renouvellement par refresh token ;
- déconnexion avec révocation du refresh token ;
- gestion des rôles `User` et `Admin` ;
- création d’un administrateur par un administrateur authentifié ;
- sélection d’une carte, achat ponctuel et abonnement ;
- intégration du widget Kkiapay pour les paiements par carte ;
- documentation Swagger de l’API en environnement de développement.

## Architecture

```text
.
├── backend/
│   └── paiementapi/              # API ASP.NET Core 8
├── frontend/
│   └── cardpaiementwithkkiapay/ # Application Angular 19
└── CardPaiementWithKkiapay.sln
```

## Technologies

<p>
  <img src="https://img.shields.io/badge/.NET%208-512BD4?logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/Angular-DD0031?logo=angular&logoColor=white" alt="Angular" />
  <img src="https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white" alt="TypeScript" />
  <img src="https://img.shields.io/badge/Node.js-339933?logo=node.js&logoColor=white" alt="Node.js" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?logo=dotnet&logoColor=white" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/Kkiapay-FF6B35?logoColor=white" alt="Kkiapay" />
</p>

## Prérequis

- .NET SDK 8 ;
- Node.js et npm ;
- une base PostgreSQL accessible ;
- un compte SMTP pour l’envoi des e-mails de confirmation ;
- une clé publique Kkiapay pour l’environnement utilisé.

## Configuration du backend

Créer le fichier `backend/paiementapi/.env` (il est ignoré par Git) :

```dotenv
CONNECTION_STRING=Host=localhost;Port=5432;Database=cardpaiement;Username=postgres;Password=mot_de_passe
JWT_KEY=une-cle-secrete-longue-et-aleatoire
JWT_ISSUER=CardPaiementWithKkiapay
JWT_AUDIENCE=CardPaiementWithKkiapay
JWT_EXPIRATION_HOURS=1
FRONTEND_URL=http://localhost:4200

EMAIL_FROM=adresse-expediteur@example.com
EMAIL_APP_PASSWORD=mot-de-passe-application
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
```

Les valeurs JWT et les identifiants SMTP ne doivent pas être commités. Le
backend arrête son démarrage si les variables obligatoires sont absentes.

## Installation et démarrage

### 1. Base de données et API

Depuis la racine du dépôt :

```bash
cd backend/paiementapi
dotnet restore
dotnet ef database update
dotnet run --launch-profile https
```

L’API est alors disponible sur `https://localhost:7169` (ou
`http://localhost:5115`). En développement, Swagger est accessible sur
`https://localhost:7169/swagger`.

Si l’outil Entity Framework n’est pas installé :

```bash
dotnet tool install --global dotnet-ef
```

Les migrations existantes se trouvent dans
`backend/paiementapi/Migrations`.

### 2. Application Angular

Dans un autre terminal :

```bash
cd frontend/cardpaiementwithkkiapay
npm install
npm start
```

Ouvrir ensuite [http://localhost:4200](http://localhost:4200).

L’URL de l’API et la clé publique Kkiapay sont configurées dans
`src/environments/environment.ts`. Pour un autre environnement, créer une
configuration Angular dédiée et ne jamais y placer de secret privé.

## Parcours utilisateur

1. Ouvrir `/register` et créer un compte.
2. Confirmer l’adresse e-mail reçue par SMTP.
3. Se connecter sur `/login`.
4. Choisir un produit puis un type de carte.
5. Effectuer un achat ponctuel ou démarrer un abonnement via Kkiapay.

Les routes `/home`, `/product`, `/purchase`, `/subscription` et `/card` sont
protégées par authentification.

## API d’authentification

Base URL : `http://localhost:5115/api/auth`

| Méthode | Route | Authentification | Description |
| --- | --- | --- | --- |
| `POST` | `/register` | Non | Créer un compte et envoyer l’e-mail de confirmation |
| `GET` | `/confirm-email?userId=...&token=...` | Non | Confirmer une adresse e-mail |
| `POST` | `/login` | Non | Obtenir un JWT et un refresh token |
| `POST` | `/refresh` | Non | Renouveler les tokens |
| `POST` | `/logout` | JWT | Révoquer un refresh token |
| `POST` | `/create-admin` | Rôle `Admin` | Créer un compte administrateur |

Exemple de connexion :

```bash
curl -X POST http://localhost:5115/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"user@example.com\",\"password\":\"MotDePasse123!\"}"
```

## Tests et build

### Backend

```bash
cd backend/paiementapi
dotnet build
```

### Frontend

```bash
cd frontend/cardpaiementwithkkiapay
npm test
npm run build
```

## Sécurité et limites actuelles

- utiliser uniquement des clés et identifiants propres à l’environnement de
  développement ou de production ;
- remplacer `sandbox: true` par la configuration de production uniquement
  après validation du parcours Kkiapay ;
- configurer une origine CORS adaptée avant un déploiement ;
- ne pas considérer les callbacks actuellement affichés dans la console
  comme une preuve serveur de paiement : une vérification côté backend est
  nécessaire pour un usage commercial.

