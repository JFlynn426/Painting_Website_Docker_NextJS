'use client'

import { Navbar, Nav, NavDropdown } from 'react-bootstrap';
import styles from './NavBar.module.css';
import Link from 'next/link';

export default function NavBar() {
    return (
        <Navbar bg="dark" variant="dark" expand="lg" className={styles.customNavbar}>
            <div className={`container-fluid ${styles.navbarContainer}`}>
                <div className="d-flex flex-column align-items-center w-100">
                    <Navbar.Brand as={Link} href="/" className={styles.customNavbarBrand}>
                        Gloria Gronowicz Fine Art
                    </Navbar.Brand>
                    <Navbar.Toggle aria-controls="navbarScroll" className={`mt-2 ${styles.navbarToggle}`} />
                    <Navbar.Collapse id="navbarScroll" className={`${styles.navbarCollapse} justify-content-center w-100`}>
                        <Nav
                            className={`${styles.customNav} navbar-nav my-2 my-lg-0 d-flex justify-content-center`}
                            style={{ maxHeight: '100px' }}
                            navbarScroll
                        >
                            <Nav.Link as={Link} href="/" className={styles.customNavLink}>
                                Home
                            </Nav.Link>
                            <Nav.Link as={Link} href="/gallery" className={styles.customNavLink}>
                                Gallery
                            </Nav.Link>
                            <Nav.Link as={Link} href="/about" className={styles.customNavLink}>
                                About
                            </Nav.Link>
                            <Nav.Link as={Link} href="/contact" className={styles.customNavLink}>
                                Contact
                            </Nav.Link>
                        </Nav>
                    </Navbar.Collapse>
                </div>
            </div>
        </Navbar>
    );
}

