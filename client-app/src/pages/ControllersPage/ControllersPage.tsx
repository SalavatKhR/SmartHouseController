import { observer } from "mobx-react";
import { Controller } from "../../components/Controller/Controller";
import { useStores } from "../../utils/Utils";
import {MainCont} from "../../containers/MainCont/MainCont";

export const ControllersPage = observer(() => {
    const { controllerStore: {controllers} } = useStores();

    return (
        <MainCont>
            <h3>Список доступных контроллеров</h3>

            <div>
                { controllers.map(controller => (
                    <Controller key={ controller.name } controller={ controller }/>
                )) }

            </div>
        </MainCont>
    )
});
