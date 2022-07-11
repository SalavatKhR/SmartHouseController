import {ControllerStore} from "../ControllerStore/ControllerStore";
import AuthStore from "../AuthStore/AuthStore";

class MainStore {
    controllerStore: ControllerStore;
    authStore: AuthStore;

    constructor() {
        this.controllerStore = new ControllerStore();
        this.authStore = new AuthStore();
    }
}

export const mainStore = new MainStore();

export default mainStore;

export { MainStore };

