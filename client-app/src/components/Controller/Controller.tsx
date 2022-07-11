import cn from "classnames/bind";
import { FC } from "react";
import { NavLink } from "react-router-dom";
import { ControllerModel } from "../../models/ControllerModel/ControllerModel";
import styles from './Controller.module.sass';


const cx = cn.bind(styles);

interface ControllerProps {
    controller: ControllerModel;
}

export const Controller: FC<ControllerProps> = ({ controller}) => {
    return (
        <div className={ styles.controller }>
            <div className={ cx(styles.controllerStatus, {
                controllerStatusActive: controller.isActive,
            }) }/>
            <div className={ styles.controllerName }>{ controller.name }</div>
            <NavLink to={`/controllers/${controller.id}`}>Перейти</NavLink>

        </div>
    )
}
