import {observable} from "mobx";
import {ControllerModel} from "../../models/ControllerModel/ControllerModel";

export class ControllerStore{
    @observable controllers: Array<ControllerModel> = [
        {
            name: 'Контроллер 1',
            isActive: true,
            id: 1,
        },
        {
            name: 'Контроллер 2',
            isActive: true,
            id: 2,
        },
        {
            name: 'Контроллер 3',
            isActive: true,
            id: 3,
        },
        {
            name: 'Контроллер 4',
            isActive: false,
            id: 4,
        }
    ];
}