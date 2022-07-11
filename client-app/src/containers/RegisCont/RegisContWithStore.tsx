import React, {useEffect} from "react";
import {useNavigate} from "react-router";
import {Input} from "../../components/ui/Input/Input";
import {Loading} from "../../components/ui/Loading/Loading";
import {Button} from "../../components/ui/Button/Button";
import {Question} from "../../components/ui/Question/Question";
import {observer} from "mobx-react";
import {useStores} from "../../utils/Utils";

export const RegisContWithStore = observer(() => {
    const {
        authStore: {
            email,
            password1,
            password2,
            emailError,
            password1Error,
            password2Error,
            inputError,
            formValid,
            isRegistered,
            setFieldValue,
            validateEmail,
            validatePassword1,
            validatePassword2
        }
    } = useStores();
    const navigate = useNavigate();

    useEffect(() => {
        if (email.length > 0 || password1.length > 0 || password2.length > 0) {
            setFieldValue(true, formValid);

        } else {
            setFieldValue(false, formValid)
        }
    }, [email, password1, password2])

    const onButtonClick = () => {
        if (emailError) {
            setFieldValue(false, formValid);
            setFieldValue('Неверный email', inputError)
        } else if (password1Error || password2Error) {
            setFieldValue(false, formValid);
            setFieldValue('Неверный пароль', inputError)
        } else {
            setFieldValue(true, formValid);
            setFieldValue('', inputError)
            setFieldValue(true, isRegistered)
            setTimeout(() => {
                setFieldValue(false, isRegistered)
                navigate('/controllers')
            }, 3000, 3001);

        }
    }

    return (
        <div className="regisCont">
            <Input type="email" value={email} onChange={validateEmail} placeholder="Адрес электронной почты"/>
            <Input type="password" value={password1} onChange={validatePassword1} placeholder="Пароль"/>
            <Input type="password" className = "last-input" value={password2} onChange={validatePassword2} placeholder="Повторите пароль"/>
            {(inputError) && <div className = "inputError">{inputError}</div>}
            {isRegistered ?
                <Loading/> :
                <Button type="submit" disabled={!formValid} onClick={onButtonClick} buttonText="Регистрация"/>
            }
            <Question question='Есть логин для входа?' href='/authorization' hrefText='Войти'/>
        </div>
    );
})