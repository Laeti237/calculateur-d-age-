# TP1 — Calculateur d'âge : Routing et MVVM

Atelier de développement mobile (.NET MAUI) — travail individuel.

Application qui calcule l'âge d'une personne à partir de son nom et de sa date de
naissance, avec une seconde page de résultat atteinte par navigation Shell, puis
une réécriture complète en **MVVM** (aucun `x:Name`, aucun `Clicked`, aucune
logique dans le code-behind).

---

## 1. Structure du dépôt

```
CalculateurAge/
├── Models/
│   └── EntreeHistorique.cs        # Une ligne d'historique (activité 6)
├── Services/
│   ├── INavigationService.cs      # Contrat de navigation (activité 6)
│   └── ShellNavigationService.cs  # Implémentation Shell (activité 6)
├── Views/
│   ├── ResultatPage.xaml          # Page de résultat
│   └── ResultatPage.xaml.cs       # Branchement QueryProperty -> ViewModel
├── ViewModels/
│   ├── BaseViewModel.cs           # INotifyPropertyChanged + SetField
│   ├── RelayCommand.cs            # ICommand synchrone (CanExecute/Rafraichir)
│   ├── AsyncRelayCommand.cs       # ICommand asynchrone (navigation)
│   ├── CalculateurViewModel.cs    # État + commandes de l'écran principal
│   └── ResultatViewModel.cs       # État + commande Retour
├── MainPage.xaml / MainPage.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs   # Routage : RegisterRoute
├── App.xaml / App.xaml.cs
├── MauiProgram.cs
├── Platforms/                     # Android, iOS, MacCatalyst, Windows
├── Properties/
├── Resources/                     # Icônes, polices, styles
├── CalculateurAge.csproj
└── README.md
```

Règle appliquée tout au long du projet : **si un ViewModel contient le mot
`Label`, `Entry`, `Button` ou `DisplayAlert`, ce n'est pas du MVVM.**

---

## 2. Historique des commits (une phase = un commit)

| Commit | Contenu |
|---|---|
| `Etape 0` | Projet .NET MAUI créé sans *sample content* (template nu) |
| `Phase A` | Version code-behind : écriture directe dans les contrôles |
| `Phase B` | Seconde page `ResultatPage`, `Routing.RegisterRoute`, `GoToAsync` avec paramètres d'URL, `[QueryProperty]`, `OnAppearing` |
| `Phase C` | Réécriture MVVM : `BaseViewModel`, `RelayCommand`, `CalculateurViewModel`, bindings uniquement |
| `Activite 6` ×3 | Fonctionnalités ajoutées (voir ci-dessous) |

```bash
git log --oneline
```

---

## 3. Les trois phases du TP

### Phase A — code-behind
`MainPage.xaml.cs` manipule directement `entryNom`, `pickerDate`, `lblResultat`.
La logique de calcul, l'interface et l'âge calculé n'existent nulle part ailleurs
que dans le texte d'un `Label`.

### Phase B — seconde page et navigation
* `Views/ResultatPage` reçoit `nom` et `age` par l'URL (`?cle=valeur`, séparateur `&`).
* `AppShell.xaml.cs` déclare la route : sans `Routing.RegisterRoute`, `GoToAsync`
  lève « route inconnue ».
* `[QueryProperty]` remplit les propriétés **après** le constructeur :
  l'affichage se fait donc dans `OnAppearing`.

| Appel | Effet |
|---|---|
| `GoToAsync(nameof(Page))` | aller vers une route enregistrée |
| `GoToAsync("..")` | revenir en arrière |
| `GoToAsync("//MainPage")` | retour à la racine |

### Phase C — MVVM
`BindingContext = new CalculateurViewModel()` : tous les `{Binding}` de la page
vont chercher leurs valeurs dans cet objet. Le bouton *Calculer* se grise tout
seul tant que `Nom` est vide (`CanExecute`), sans une seule ligne d'interface.

---

## 4. Fonctionnalités ajoutées (Activité 6)

Toutes sont en MVVM : aucune logique dans le code-behind.

1. **Statut Majeur / Mineur** — propriété `Statut` calculée après l'âge.
2. **Commande Effacer** — `EffacerCommand` remet tous les champs à zéro
   (`CanExecute` : le bouton se grise une fois tout vide).
3. **Refus d'une date future** — message d'erreur en rouge (`Erreur`,
   `ErreurVisible`) au lieu d'un `DisplayAlert`.
4. **Jours restants avant le prochain anniversaire** — propriété
   `ProchainAnniversaire` (gère le 29 février via `DateTime.AddYears`).
5. **Historique des calculs** — `ObservableCollection<EntreeHistorique>` liée à un
   `CollectionView`, compteur `CompteHistorique`, commande `EffacerHistoriqueCommand`.
6. **Résultat remonté vers `ResultatPage` par le ViewModel** — le ViewModel ne
   connaît ni Shell ni l'URL : il appelle `INavigationService.VersResultatAsync(...)`
   (`AfficherResultatCommand`). `ResultatPage` ne fait plus que brancher
   `[QueryProperty]` sur `ResultatViewModel`.

---

## 5. Construction et exécution

Prérequis : .NET SDK 10 + charge de travail `maui` (et `maui-android` pour
l'émulateur).

```bash
# Windows (desktop)
dotnet build CalculateurAge.csproj -f net10.0-windows10.0.19041.0
dotnet run   --project CalculateurAge.csproj -f net10.0-windows10.0.19041.0

# Android (émulateur)
dotnet build CalculateurAge.csproj -t:Run -f net10.0-android
```

Sous Visual Studio : sélectionner la cible *Windows Machine* ou un émulateur
Android, puis `F5`. Vérifier au premier lancement que l'application s'ouvre sur
le formulaire *Calculateur*.

---

## 6. Publication sur GitHub

```bash
git remote add origin https://github.com/<votre-compte>/CalculateurAge.git
git push -u origin main
```

Dépôt **public**, historique complet (étape 0 → phases A/B/C → activités 6).
La vidéo de démonstration (30 s max) montre : calcul, statut, compte à rebours,
date future refusée, effacement, historique et navigation vers la page de résultat.

---

## 7. Erreurs fréquentes rencontrées

| Symptôme | Cause |
|---|---|
| Écran vide, aucune erreur | Nom de propriété mal orthographié dans `{Binding}` |
| La valeur ne se met pas à jour | Setter qui affecte le champ sans appeler `SetField` |
| Route inconnue à l'exécution | `Routing.RegisterRoute` oublié dans `AppShell.xaml.cs` |
| Page de détail vide | Affichage dans le constructeur au lieu de `OnAppearing` |
| Le bouton reste toujours grisé | `Rafraichir()` non appelé depuis le setter de `Nom` |
| Erreur de compilation sur `x:Class` | Namespace du fichier qui ne correspond pas au dossier |
