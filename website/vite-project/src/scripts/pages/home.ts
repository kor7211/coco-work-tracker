import "../../styles/home.css";

import "../components/font-setter.ts";
import "../components/i18n_setter.ts";
import "../components/i18n_button.ts";

import { Navi } from "../animations/navi.ts";

const navi = new Navi();
navi.startAnimation(navi.createIdleAnimation());
const wind: HTMLElement = document.getElementById("character-window") as HTMLElement;
wind.append(navi.svg);