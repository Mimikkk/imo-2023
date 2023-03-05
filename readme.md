# Laboratorium - Inteligentne metody optymalizacji

## Zadanie 1 - Heurystyki konstrukcyjne

W ramach zadania należało:

- [x] Zaimplementować zapis do obrazu.
- [x] Zaimplementować wczytywanie instancji kroa100 i krob100.
    - [ ] Pseudokod
    - [ ] Opis
- [x] Zaimplementować algorytm zachłanny inspirowany najbliższego sąsiada.
    - [ ] Pseudokod
    - [ ] Opis
- [x] Zaimplementować algorytm zachłanny inspirowany metodą rozbudowy cyklu.
    - [ ] Pseudokod
    - [ ] Opis
- [ ] Zaimplementować algorytm zachłanny typu heurestyki z żalem na bazie algorytmu inspirowanego metodą rozbudowy cyklu
  przy wykorzystaniu 2-żal (2-regret).
    - [ ] Pseudokod
    - [ ] Opis
- [ ] Eksperymenty obliczeniowe na każdej instancji poprzez 100 krotne uruchomienie algorytmu i przedstawienie wyników.

## Eksperyment

Poprzez eksperymenty obliczeniowe chcemy sprawdzić, które z heurystyk konstrukcyjnych odnajduje najlepsze rozwiązanie. W
tym celu zostało wykonane stukrotne uruchomienie każdego z algorytmów na każdej z wybranych instancji ( kroA100,
kroB100 ).

Jako miara jakości rozwiązania wybrano długość ścieżki, która została obliczana poprzez sumę długości między węzłami w
odnalezionym cyklu.

### KroA100

| Algorytm | Długość cyklu A | Długość cyklu B |
|----------|-----------------|-----------------|
|          |                 |                 |
|          |                 |                 |

#### Najlepszy wynik

<div style="
    display: flex;
    justify-content: center;
    align-items: center;
">
<img src="imo-2023/Resources/Graphs/kroA100-greedy-nearest-neighbour.png" alt="wyniki przedstawiające 2 cykle">
</div>

### KroB100

| Algorytm | Długość cyklu A | Długość cyklu B |
|----------|-----------------|-----------------|
|          |                 |                 |
|          |                 |                 |

#### Najlepszy wynik

<div style="
    display: flex;
    justify-content: center;
    align-items: center;
">
<img src="./imo-2023/Resources/Graphs/kroA100-greedy-nearest-neighbour.png" alt="wyniki przedstawiające 2 cykle">
</div>
