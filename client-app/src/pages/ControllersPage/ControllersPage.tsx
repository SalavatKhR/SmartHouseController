import { observer } from "mobx-react";
import { Controller } from "../../components/Controller/Controller";
import { useStores } from "../../utils/Utils";
import {MainCont} from "../../containers/MainCont/MainCont";

export const ControllersPage = observer(() => {
    const { controllerStore: {addController, removeController, controllers} } = useStores();

    const handleAddController = (): void => {
        const randomNumber = Math.floor(Math.random() * (10000 - 1) + 1);

        addController({
            id: randomNumber,
            name: `Контроллер ${ randomNumber }`,
            isActive: true,
        });
    };

    const onDelete = (id: number): void => {
        removeController(id);
    };


    return (
        <MainCont>
            <h3>Список доступных контроллеров</h3>

            <div>

                { controllers.map(controller => (
                    <Controller key={ controller.name } controller={ controller } onDelete={ onDelete }/>
                )) }

                {/*<button onClick={ handleAddController }>Добавить контроллеры</button>*/}
            </div>
        </MainCont>
    )
});
