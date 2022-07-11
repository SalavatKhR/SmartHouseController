import React from 'react';
import logo from "../../../assets/icons/logo.svg";
import {RegisContWithStore} from "../../../containers/RegisCont/RegisContWithStore";
import styles from '../Logo-style.module.sass'


export function Regis() {

    return (
        <div className={styles.regis}>
            <img src={logo} className={styles.logo} alt="logo"/>
            <RegisContWithStore/>
        </div>
    );
}

