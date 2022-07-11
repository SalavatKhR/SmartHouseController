import NavigationBar from "./NavigationBar";
const Layout = ({ children }) => {
    return (
        <>
            <NavigationBar/>
            <main>{children}</main>
        </>
    );
};

export default Layout;