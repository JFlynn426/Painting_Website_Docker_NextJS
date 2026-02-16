'use client'

import { Navbar, Nav, NavDropdown } from 'react-bootstrap';
import styles from './NavBar.module.css';
import Link from 'next/link';
import { paintingCategories } from './models/paintingCategories';

export default function NavBar() {
    return (
        <Navbar bg="dark" variant="dark" expand="lg" className={styles.customNavbar} sticky="top">
            <div className={styles.navbarContainer}>
                <div className="d-flex justify-content-between align-items-center w-100">
                    <Navbar.Brand as={Link} href="/" className={styles.customNavbarBrand}>
                        Gloria Gronowicz Fine Art
                    </Navbar.Brand>
                    <Navbar.Toggle aria-controls="navbarScroll" className={`${styles.navbarToggle}`} />
                </div>
                <Navbar.Collapse id="navbarScroll" className={`${styles.navbarCollapse} justify-content-center w-100`}>
                    <Nav
                        className={`${styles.customNav} navbar-nav my-2 my-lg-0 d-flex justify-content-center`}
                        navbarScroll
                    >
                        <Nav.Link as={Link} href="/" className={styles.customNavLink}>
                            Home
                        </Nav.Link>
                        <Nav.Link as={Link} href="/about" className={styles.customNavLink}>
                            About
                        </Nav.Link>
                        <NavDropdown
                            title="Paintings"
                            id="paintings-dropdown"
                            className={styles.customNavDropdown}
                            menuVariant="dark"
                        >
                            {paintingCategories.map((category) => (
                                <NavDropdown.Item
                                    as={Link}
                                    href={`/paintings/${category.slug}`}
                                    key={category.id}
                                    className={styles.customNavDropdownItem}
                                >
                                    {category.name}
                                </NavDropdown.Item>
                            ))}
                        </NavDropdown>
                        <Nav.Link as={Link} href="/new_paintings" className={styles.customNavLink}>
                            New Paintings
                        </Nav.Link>
                        <Nav.Link as={Link} href="/gallery" className={styles.customNavLink}>
                            Gallery
                        </Nav.Link>
                        <Nav.Link as={Link} href="/contact" className={styles.customNavLink}>
                            Contact
                        </Nav.Link>
                    </Nav>
                </Navbar.Collapse>
            </div>
        </Navbar>
    );
}