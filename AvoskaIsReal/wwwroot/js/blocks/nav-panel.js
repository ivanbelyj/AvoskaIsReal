$(function () {
  navPanel = $(".nav-panel");
  button = $(".nav-panel__button");
  burger = $(".nav-panel__burger");
  body = $(".body");

  button.click(onClick);
});

let navPanel;
let button;
let burger;
let body;
let overflowLastValue;

function onClick() {
  burger.toggleClass("burger_closed");
  navPanel.toggleClass("nav-panel_closed");
  button.toggleClass("nav-panel__button_opened");
  if (body.css("overflow") == "hidden") {
    body.css({ overflow: overflowLastValue });
  } else {
    overflowLastValue = body.css("overflow");
    body.css({ overflow: "hidden" });
  }
}
