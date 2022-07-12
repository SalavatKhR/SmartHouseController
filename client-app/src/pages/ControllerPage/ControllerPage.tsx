import { observer } from "mobx-react";
import { FC, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useStores } from "../../utils/Utils";
import {ControllerModel} from "../../models/ControllerModel/ControllerModel";
import {MainCont} from "../../containers/MainCont/MainCont";

export type Params = {
    id: string;
}

export const ControllerPage: FC = observer(() => {
    const { id } = useParams<Params>();
    const [controller, setController] = useState<ControllerModel>();
    const { controllerStore: { controllers } } = useStores();

    useEffect(() => {
        // @ts-ignore
        const controller = controllers.find(controller => controller.id === +id);

        setController(controller);
    }, [id, controllers]);

    return (
        <MainCont>
            { controller && (
                <ul>
                    <li>Название: {controller.name}</li>
                    <li>
                        {controller.isActive ? 'Активный' : 'Неактивный'}
                    </li>
                </ul>
            )}
        </MainCont>
    )
});
