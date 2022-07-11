import React from "react";
import styles from "./Loading.module.sass"

export const Loading = () => {
    return (
        <div className ={styles.floatingBarsG}>
            <div className={styles.blockG} id="rotateG_01"/>
            <div className={styles.blockG} id="rotateG_02"/>
            <div className={styles.blockG} id="rotateG_03"/>
            <div className={styles.blockG} id="rotateG_04"/>
            <div className={styles.blockG} id="rotateG_05"/>
            <div className={styles.blockG} id="rotateG_06"/>
            <div className={styles.blockG} id="rotateG_07"/>
            <div className={styles.blockG} id="rotateG_08"/>
        </div>
    );
}