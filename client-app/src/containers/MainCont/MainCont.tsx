import React, { FC, ReactNode } from "react";
import { NavLink } from 'react-router-dom';
import styles from './MainCont.module.sass';
import logo from "../../assets/icons/logo.svg";

interface MainContProps {
    children: ReactNode;
}

export const MainCont: FC<MainContProps> = ({children}) => {
    return (
        <div className={ styles.container }>
            <header className={ styles.header }>
                <img src={logo} className={styles.logo} alt="logo"/>
            </header>

            <main>
                {children}
            </main>
        </div>
    )
}
