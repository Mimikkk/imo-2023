# Laboratorium - Inteligentne metody optymalizacji

## Zadanie 1 - Heurystyki konstrukcyjne

W ramach zadania należało:

- [x] Zaimplementować zapis do obrazu.
- [x] Zaimplementować wczytywanie instancji kroa100 i krob100.
- [x] Zaimplementować algorytm zachłanny inspirowany najbliższego sąsiada.
- [x] Zaimplementować algorytm zachłanny inspirowany metodą rozbudowy cyklu.
- [x] Zaimplementować algorytm zachłanny typu heurestyki z żalem na bazie algorytmu inspirowanego metodą rozbudowy cyklu
  przy wykorzystaniu 2-żal (2-regret).
- [x] **Dodatkowo** Zaimplementować algorytm zachłanny typu heurestyki z żalem na bazie algorytmu inspirowanego metodą
  rozbudowy cyklu
  przy wykorzystaniu 2-żal (2-regret) oraz uwzględnieniem wag.
- [x] Eksperymenty obliczeniowe na każdej instancji poprzez 100 krotne uruchomienie algorytmu i przedstawienie wyników.

### Dodatek

W ramach zadania studenci również zaimplementowali interfejs graficzny do wyświetlania i manipulowania wynikami.

- Interfejs pozwala na wybór instancji, algorytmu, parametrów algorytmu, zapis wyniku do pliku.
- Parametryzacji podlega wielkość populacji oraz wagi takie jak k-żal, waga dodatkowa żalu.
- Wszystkie algorytmy zostały zaimplementowane również w 3 wersjach.
    - Wersja Single ( pojedyńczy agent )
    - Wersja Double ( oczekiwana - dwóch agentów )
    - Wersja Multiple ( wielu agentów )

## Implementacja zapisu obrazu.

Obrazy są zapisywane do postaci png za pomocą biblioteki ScottPlot.

- Miejsce zapisu to kotalog `imo-2023/Resources/Graphs/`.
- Odbywa się za pomocą metody `Save` na obiekcie `Plot`.

## Implementacja wczytywania instancji

Wczytywanie instancji odbywa się za pomocą metody `Read` w klasie `Instance`.

- Pliki są odczytywane z katalogu `imo-2023/Resources/Instances/`.
- Wczytywane są tylko współrzędne węzłów.
- Po zaczytaniu pliku, tworzona jest macierz odległości między węzłami służąca do prekalkulacji obliczeń.

## Implementacja algorytmu zachłannego inspirowanego metodą najbliższego sąsiada.

### Pseudokod

- Wybierz losowo lub na podstawie przekazanego indeksu wierzchołek startowy pierwszej ścieżki.
- Wybierz wierzchołek najdalszy do pierwszego wierzchołka pierwszej ścieżki.
- **powtarzaj**
    - Dodaj do pierwszej ścieżki rozwiązania wierzchołek (i prowadzącą do niego krawędź) najbliższy, który nie jest
      zawarty w pierwszej ani drugiej ścieżce.
    - Dodaj do drugiej ścieżki rozwiązania wierzchołek (i prowadzącą do niego krawędź) najbliższy, który nie jest
      zawarty w pierwszej ani drugiej ścieżce.
- **dopóki** nie zostały dodane wszystkie wierzchołki.
- Dodaj do pierwszej ścieżki krawędź z ostatniego do pierwszego wierzchołka aby utworzyć wynikowy pierwszy cykl.
- Dodaj do drugiej ścieżki krawędź z ostatniego do pierwszego wierzchołka aby utworzyć wynikowy drugi cyl.

### Opis

Początkowo są wybierane 2 elementy o skrajnej odległości według prekalkulowanej macierzy odległości.
Następnie dodawane są kolejne elementy, które są najbliżej wybranego elementu.
Dodawanie elementów jest wykonywane dla obu ścieżek w kolejności pierwsza ścieżka, druga ścieżka.
Element najbliższy do ścieżki jest odnajdowany przez najmniejszą wartość odległości do pierwszego lub ostatniego
elementu w macierzy odległości dla elementów niezawartych w pierwszej ani drugiej ścieżce.
Po wybraniu element jest dodawany do początku lub końca ścieżki na podstawie porównania odległości do pierwszego i
ostatniego elementu w ścieżce. Jeżeli element jest bliższy do pierwszego elementu w ścieżce to jest dodawany na
początek, w przeciwnym wypadku na koniec.
I tak się dzieje do momentu, gdy wszystkie elementy zostaną dodane do ścieżek. Po czym tworzony jest cykl z
ostatnich elementów do pierwszych.

## Implementacja algorytmu zachłannego inspirowanego metodą rozbudowy cyklu.

### Pseudokod

- Wybierz losowo lub na podstawie przekazanego indeksu wierzchołek startowy pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do pierwszego wierzchołka pierwszej ścieżki i dodaj do pierwszej ścieżki.
- Utwórz początek drugiej ścieżki przez wybór wierzchołka najdalszego do pierwszego wierzchołka pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do ostatniego wierzchołka drugiej ścieżki i dodaj do drugiej ścieżki.
- **powtarzaj**
    - Wstaw do pierwszego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości pierwszego cyklu.
    - Wstaw do drugiego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości drugiego cyklu.
- **dopóki** nie zostały dodane wszystkie wierzchołki.

### Opis

Początkowo jest wybierany element losowy lub na podstawie predefiniowanego indeksu i dodawany do pierwszego cyklu.
Następnie jest wybierany najbliższy element do pierwszego elementu i dodawany do pierwszego cyklu.
Najbliższy element to taki, który ma najmniejszą wartość w prekalkulowanej macierzy odległości i nie jest elementem
zabronionym ( na początku pierwszy element ).
Następnie jest wybierany element najdalszy do pierwszego elementu i dodawany do drugiego cyklu. Po tym jest wybierany
najbliższy
element do ostatniego elementu drugiego cyklu i dodawany do ścieżki. Element najbliższy jest definiowany analogicznie
przez najmniejszą
wartość w macierzy odległości i nie jest elementem zabronionym ( elementy cyklu pierwszego i element pierwszy cyklu
drugiego ).
Później cykle są rozbudowywane w taki sposób, że wstawiany jest element, który nie należy do żadnego z cykli i powoduje
najmniejszy wzrost
długości cyklu. Element do wstawienia jest wybierany na podstawie przejrzenia wszystkich nieprzypisanych do cykli
wierzchołków i wybrania
takiego, który po przejrzeniu każdej z krawędzi cyklu powoduje najmniejszy wzrost długości. Obliczenie wzrostu cyklu
jest dokonywane przez
dodanie do długości cyklu długości dwóch krawędzi do a i b oraz odjętą od tej wartości długość krawędzi między a i b. Ta
wartość jest
minimalizowana. I tak się dzieje do momentu, gdy wszystkie elementy zostaną dodane do cykli.

## Implementacja algorytmu zachłannego typu heurestyki z żalem na bazie algorytmu inspirowanego metodą rozbudowy cyklu przy wykorzystaniu 2-żal (2-regret)

### Pseudokod

- Wybierz losowo lub na podstawie przekazanego indeksu wierzchołek startowy pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do pierwszego wierzchołka pierwszej ścieżki i dodaj do pierwszej ścieżki.
- Utwórz początek ścieżki przez wybór wierzchołka najdalszego do pierwszego wierzchołka pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do ostatniego wierzchołka drugiej ścieżki i dodaj do drugiej ścieżki.
- **powtarzaj**
    - Wstaw do pierwszego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości pierwszego cyklu, a jego żal między opcjami jest największy.
    - Wstaw do drugiego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości drugiego cyklu, a jego żal między opcjami jest największy.
- **dopóki** nie zostały dodane wszystkie wierzchołki.

### Opis

Początkowo jest wybierany element losowy lub na podstawie predefiniowanego indeksu i dodawany do pierwszego cyklu.
Następnie jest wybierany najbliższy element do pierwszego elementu i dodawany do pierwszego cyklu.
Najbliższy element to taki, który ma najmniejszą wartość w prekalkulowanej macierzy odległości i nie jest elementem
zabronionym ( na początku pierwszy element ).
Następnie jest wybierany element najdalszy do pierwszego elementu i dodawany do drugiego cyklu. Po tym jest wybierany
najbliższy
element do ostatniego elementu drugiego cyklu i dodawany do ścieżki. Element najbliższy jest definiowany analogicznie
przez najmniejszą
wartość w macierzy odległości i nie jest elementem zabronionym ( elementy cyklu pierwszego i element pierwszy cyklu
drugiego ).
Później cykle są rozbudowywane w taki sposób, że wstawiany jest element, który nie należy do żadnego z cykli i powoduje
najmniejszy wzrost
długości cyklu. Element do wstawienia jest wybierany na podstawie przejrzenia wszystkich nieprzypisanych do cykli
wierzchołków i wybrania
takiego, który po przejrzeniu każdej z krawędzi cyklu powoduje najmniejszy wzrost żalu. Obliczenie żalu opcji odbywa się
przez odnalezienie wszystkich opcji wstawienia, posortowania ich kosztu jak w metodzie rozbudowywania cyklu, a następnie
wyliczenie wartości przez
sumę różnic pierwszego z posortowanych elementów o k-1 kolejne. Ta wartość jest minimalizowana i na podstawie tej
sortowana jest przestrzeń opcji,
po czym wybierana jest pierwsza opcja ( najmniejszy żal ).
I tak się dzieje do momentu, gdy wszystkie elementy zostaną dodane do cykli.

## Implementacja algorytmu zachłannego typu heurestyki z żalem na bazie algorytmu inspirowanego metodą rozbudowy cyklu przy wykorzystaniu 2-żal (2-regret) oraz uwzględnieniem wag.

### Pseudokod

- Wybierz losowo lub na podstawie przekazanego indeksu wierzchołek startowy pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do pierwszego wierzchołka pierwszej ścieżki i dodaj do pierwszej ścieżki.
- Utwórz początek ścieżki przez wybór wierzchołka najdalszego do pierwszego wierzchołka pierwszej ścieżki.
- Wybierz wierzchołek najbliższy do ostatniego wierzchołka drugiej ścieżki i dodaj do drugiej ścieżki.
- **powtarzaj**
    - Wstaw do pierwszego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości pierwszego cyklu, a jego żal między opcjami jest największy.
    - Wstaw do drugiego cyklu w najlepsze miejsce wierzchołek, który nie należy do cyklu pierwszego, ani drugiego
      powodujący najmniejszy wzrost długości drugiego cyklu, a jego żal między opcjami jest największy.
- **dopóki** nie zostały dodane wszystkie wierzchołki.

### Opis

Początkowo jest wybierany element losowy lub na podstawie predefiniowanego indeksu i dodawany do pierwszego cyklu.
Następnie jest wybierany najbliższy element do pierwszego elementu i dodawany do pierwszego cyklu.
Najbliższy element to taki, który ma najmniejszą wartość w prekalkulowanej macierzy odległości i nie jest elementem
zabronionym ( na początku pierwszy element ).
Następnie jest wybierany element najdalszy do pierwszego elementu i dodawany do drugiego cyklu. Po tym jest wybierany
najbliższy
element do ostatniego elementu drugiego cyklu i dodawany do ścieżki. Element najbliższy jest definiowany analogicznie
przez najmniejszą
wartość w macierzy odległości i nie jest elementem zabronionym ( elementy cyklu pierwszego i element pierwszy cyklu
drugiego ).
Później cykle są rozbudowywane w taki sposób, że wstawiany jest element, który nie należy do żadnego z cykli i powoduje
najmniejszy wzrost
długości cyklu. Element do wstawienia jest wybierany na podstawie przejrzenia wszystkich nieprzypisanych do cykli
wierzchołków i wybrania
takiego, który po przejrzeniu każdej z krawędzi cyklu powoduje najmniejszy wzrost żalu wraz z dodatkową wagą najlepszego
rozszerzenia cyklu.
Obliczenie żalu opcji odbywa się przez odnalezienie wszystkich opcji wstawienia, posortowania ich kosztu jak w metodzie
rozbudowywania cyklu, a następnie
wyliczenie wartości przez
sumę różnic pierwszego z posortowanych elementów o k-1 kolejne. Ta wartość jest minimalizowana i na podstawie tej
sortowana jest przestrzeń opcji.
po czym wybierana jest pierwsza opcja ( najmniejszy żal plus waga z najlepszej opcji włożenia ).
I tak się dzieje do momentu, gdy wszystkie elementy zostaną dodane do cykli.

## Eksperyment

Poprzez eksperymenty obliczeniowe chcemy sprawdzić, które z heurystyk konstrukcyjnych odnajduje najlepsze rozwiązanie. W
tym celu zostało wykonane stukrotne uruchomienie każdego z algorytmów na każdej z wybranych instancji ( kroA100,
kroB100 ).

Jako miara jakości rozwiązania wybrano długość ścieżki, która została obliczana poprzez sumę długości między węzłami w
odnalezionym cyklu.

# Zadanie 2 - Heurestyki lokalne

W ramach zadania należało:

- [ ] Zaimplemnetować algorytm losowego zachłannego adapcyjnego przeszukiwania ( _GRASP_ ).
- [ ] Zaimplementować algorytm przeszukiwania lokalnego w wersji stromej ( _steepest_ ).
    - [ ] Przy wykorzystaniu sąsiedztwa z wymianą wierzchołków między cyklami.
    - [ ] Przy wykorzystaniu sąsiedztwa z wymianą wierzchołków wewnątrz cyklu.
    - [ ] Zaczynającą od rozwiązania uzyskanego w algorytmie zachłannym.
    - [ ] Zaczynający od rozwiązania uzyskanego w algorytmie losowym ( grasp ). - [ ] Zaimplementować algorytm przeszukiwania lokalnego w wersji zachłannej ( _greedy_ ).
    - [ ] Przy wykorzystaniu sąsiedztwa z wymianą wierzchołków między cyklami.
    - [ ] Przy wykorzystaniu sąsiedztwa z wymianą wierzchołków wewnątrz cyklu.
    - [ ] Zaczynającą od rozwiązania uzyskanego w algorytmie zachłannym.
    - [ ] Zaczynający od rozwiązania uzyskanego w algorytmie losowym ( grasp ).

## Implementacja algorytmu losowego zachłannego adapcyjnego przeszukiwania ( _GRASP_ ).

### Pseudokod

### Opis

## Implementacja algorytmu przeszukiwania lokalnego w wersji stromej ( _steepest_ ).

### Wersja z wymianą wierzchołków między cyklami

#### Pseudokod

#### Opis

### Wersja z wymianą wierzchołków wewnątrz cyklu

#### Pseudokod

#### Opis

### Wersja zaczynająca od rozwiązania uzyskanego w algorytmie zachłannym

#### Pseudokod

#### Opis

### Wersja zaczynająca od rozwiązania uzyskanego w algorytmie losowym ( grasp )

#### Pseudokod

#### Opis

## Implementacja algorytmu przeszukiwania lokalnego w wersji zachłannej ( _greedy_ ).

### Wersja z wymianą wierzchołków między cyklami

#### Pseudokod

#### Opis

### Wersja z wymianą wierzchołków wewnątrz cyklu

#### Pseudokod

#### Opis

### Wersja zaczynająca od rozwiązania uzyskanego w algorytmie zachłannym

#### Pseudokod

#### Opis

### Wersja zaczynająca od rozwiązania uzyskanego w algorytmie losowym ( grasp )

#### Pseudokod

#### Opis

## Eksperyment

Poprzez eksperymenty obliczeniowe chcemy sprawdzić, które z heurystyk lokalnego przeszukiwania odnajduje najlepsze
rozwiązanie. W
tym celu zostało wykonane stukrotne uruchomienie każdego z algorytmów na każdej z wybranych instancji ( kroA100,
kroB100 ).

Jako miara jakości rozwiązania wybrano długość ścieżki, która została obliczana poprzez sumę długości między węzłami w
odnalezionym cyklu.