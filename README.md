# Приложение для создания коллажа

Данное приложение генерирует коллаж из нескольких изображений определённой ширины.

Приложение было разработано в рамках моего портфолио для демонстрации навыков в разработке на C#.

## Описание
Текстовое поле приложения предназначено для ввода команд заполнения коллажа с определенным синтаксисом. С помощью данных команд можно определить столбцы м строки (контейнеры), и изображения коллажа, а также их взаимосвязь.
По введенным командам строится иерархическое дерево заполнения.

Синтаксис команд:
- Определение начального элемента: 0 root тип_контейнера
- Определение контейнера: тип_контейнера название_контейнера название_родительского_контейнера
- Определение изображения: i название_родительского_контейнера путь_к_файлу

Тип контейнера:
- c - столбец
- r - строка

## Скриншот
![alt text](https://github.com/AugustLC/Collage/blob/main/screenshots/1.png)
