$(function () {
  limitedTexts = Array.from($(".limited-text")).map(
    (item) => new LimitedText($(item), symbolsCountLimit)
  );
});

const symbolsCountLimit = 160;
let limitedTexts;

class LimitedText {
  constructor(element, maxSymbolsCount) {
    this.element = element;
    this.maxSymbolsCount = maxSymbolsCount;

    this.update();
  }
  addExpandButton() {
    if (!this.expandButton) {
      this.expandButton = $("<div>")
        .text("Развернуть")
        .addClass("limited-text__button");
      this.expandButton.click(function () {});
      this.expandButton.appendTo(this.element);

      const toggleFunctionBinded = this.toggle.bind(this);
      this.expandButton.click(function () {
        toggleFunctionBinded();
      });
    }
  }
  removeExpandButton() {
    this.expandButton.remove();
    this.expandButton = null;
  }

  // Вызывается в начале, либо при изменении текста извне
  update() {
    this.contentElement = this.element.find(".limited-text__content");

    // Текст для возможности восстановления
    this.initialText = this.contentElement.text().trim();

    // Добавление кнопки при ее отсутствии, если она нужна
    if (this.initialText.length > this.maxSymbolsCount) {
      this.addExpandButton();
    } else {
      return;
    }

    this.limitedText = this.getLimitedString(this.initialText);
    console.log(this.limitedText, this.initialText);

    this.contentElement.text(this.limitedText);
    this.isExpanded = false;
  }

  getLimitedString(str) {
    if (this.maxSymbolsCount < 4) return "...";
    const limited = str.slice(0, this.maxSymbolsCount - 3) + "...";
    return limited;
  }
  toggle() {
    this.contentElement.text(
      this.isExpanded
        ? /* Если открыт, закрытие */ this.limitedText
        : this.initialText
    );
    this.isExpanded = !this.isExpanded;

    // Обновление кнопки
    this.expandButton.text(this.isExpanded ? "Свернуть" : "Развернуть");
  }
}
