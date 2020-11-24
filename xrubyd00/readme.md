# Dokumentace IPK projektu č.1 (2019/2020)
## autor: David Rubý (xrubyd00)
K implementaci tohoto projektu jsem použil jazyk C# s platformou .NET
Řešení se skládá z několika částí:
1. Smyčka, která opakovaně přijímá requesty ze strany klienta.
2. Ve smyčce je umístěn ``Socket handler``, na kterém se řízení. programu zastaví a čeká, dokud není navázano spojení ze strany klienta.
3. Jakmile je spojení navazáno, ``handler`` příjme data ve formě bitů, které se následně přeloží do řetězce.
4. Řetězec se rozdělí podle nových řádků do pole těchto řádků.
5. První řádek (hlavička HTTP requestu) se rozdělí podle mezer do pole. První položka v tomto poli je tedy metoda, kterou se data posílají. ``GET`` nebo ``POST`` a podle nich se mění tok programu.

>### GET
>Pomocí requlárního výrazu zjistí, o jaký typ požadavku se jedná. >(**A** nebo **PTR**)
>> #### typ A
>> 1. Request se rozparsuje a získá z něj podobu domenového jména.
>> 2. Pomocí **DNS** třídy se pokusí jméno přeložit na IP adresu a vygenerovat hlavičku a tělo **HTTP** odpovědi.
>> 3. Při chybě při překladu se jako hlavička odpovědi použije ``HTTP/1.1 404 Not Found``.
>
>> #### typ PTR
>> 1. Request se rozparsuje a získá se z něj podova adresy.
>> 2. Pomocí **DNS** třídy se pokusí adresu přeložit na doménové jméno a vygenerovat hlavičku a tělo **HTTP** odpovědi.
>> 3. Při chybe při překladu se jako hlavička odpovědi použije ``HTTP/1.1 404 Not Found``.
>### POST
>Pomocí requlárního výrazu zjistí, o jaký typ požadavku se jedná. (**A** nebo **PTR**)
>> #### typ A
>> 1. Request se rozparsuje a získá z něj podobu domenového jména.
>> 2. Pomocí **DNS** třídy se pokusí jméno přeložit na IP adresu a vygenerovat hlavičku a tělo **HTTP** odpovědi.
>> 3. Při chybě při překladu se jako hlavička odpovědi použije ``HTTP/1.1 404 Not Found``.
>
>> #### typ PTR
>> 1. Request se rozparsuje a získá se z něj podova adresy.
>> 2. Pomocí **DNS** třídy se pokusí adresu přeložit na doménové jméno a vygenerovat hlavičku a tělo **HTTP** odpovědi.
>> 3. Při chybe při překladu se jako hlavička odpovědi použije ``HTTP/1.1 404 Not Found``.
>### Jiná metoda
>Jako hlavička **HTTP** odpovědi se použije ``HTTP/1.1 405 Method Not Allowed``.

6. Hlavička a tělo **HTTP** odpovědi spojí dohromady a přeloží do podoby bitů.
7. Bity se pošlou zpět klientovi.
8. Uzavře se spojení.
9. Smyčka iteruje od začátku, kde čeká na další spojení.